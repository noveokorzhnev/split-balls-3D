using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IInteractWithBallOnEnter
{
    void OnBallEnters(Ball ball);
}

public interface IInteractWithBallOnExit
{
    void OnBallExits(Ball ball);
}

public class GameInteractionsSystem : IGameplaySystem, IFinishTrigger, IBallDestroyer, IKeyCollector
{
    // Colliders handling chain of responsibility.
    private abstract class TriggerHandler<T>
    {
        private TriggerHandler<T> successor;

        private System.Func<Collider, T> getActor;
        protected abstract void HandleBall(T actor, Ball ball);

        protected TriggerHandler (System.Func<Collider, T> getActor, TriggerHandler<T> successor)
        {
            this.getActor = getActor;
            this.successor = successor;
        }

        public void Handle(Ball ball, Collider trigger)
        {
            var actor = getActor(trigger);

            if (actor != null)
            {
                HandleBall(actor, ball);
            }
            else
            {
                if (successor != null)
                {
                    successor.Handle(ball, trigger);
                }
            }
        }
    }

    private class OnTriggerEnterHandler : TriggerHandler<IInteractWithBallOnEnter>
    {
        public OnTriggerEnterHandler(System.Func<Collider, IInteractWithBallOnEnter> getActor, OnTriggerEnterHandler successor) : 
            base(getActor, successor) { }

        public OnTriggerEnterHandler(IEnumerable<InteractableStageEntity> stageEntities, OnTriggerEnterHandler successor) : 
            base((trigger) => stageEntities.FirstOrDefault(x => trigger == x.Collider), successor) { }

        protected override void HandleBall(IInteractWithBallOnEnter actor, Ball ball)
        {
            actor.OnBallEnters(ball);
        }
    }

    private class OnTriggerExitHandler : TriggerHandler<IInteractWithBallOnExit>
    {
        public OnTriggerExitHandler(System.Func<Collider, IInteractWithBallOnExit> getActor, OnTriggerExitHandler successor) : 
            base(getActor, successor) { }

        public OnTriggerExitHandler(IEnumerable<InteractableStageEntity> stageEntities, OnTriggerExitHandler successor) :
           base((trigger) => stageEntities.FirstOrDefault(x => trigger == x.Collider), successor)
        { }

        protected override void HandleBall(IInteractWithBallOnExit actor, Ball ball)
        {
            actor.OnBallExits(ball);
        }
    }


    private BallSystem ballSystem;

    private Multiplier[] multipliers;
    private Lock[] locks;
    private LocalWindZone[] windZones;
    private Portal[] portals;

    private Key key;

    private Collider finishCollider;
    private Collider destroyCollider;

    public event System.Action onFinishTriggered;
    public event System.Action onBallDestroyed;
    public event System.Action onKeyCollected;

    private event System.Action<Ball, Collider> handleTriggerEnter;
    private event System.Action<Ball, Collider> handleTriggerExit;

    private OnTriggerEnterHandler onTriggerEnterChain;
    private OnTriggerExitHandler onTriggerExitChain;

    public GameInteractionsSystem(BallSystem ballSystem, KeysSystem keysSystem, StageSystem stageSystem)
    {
        this.ballSystem = ballSystem;
        stageSystem.AddFinishTrigger(this);
        ballSystem.AddBallDestroyer(this);
        keysSystem.AddKeyCollector(this);

        var gamezone = Object.FindObjectOfType<GameZone>();
        finishCollider = gamezone.FinishTrigger;
        destroyCollider = gamezone.DestroyTrigger;

        handleTriggerEnter += (ball, trigger) =>
        {
            bool isMainTrigger = TestMainTriggers(ball, trigger);
            if (!isMainTrigger)
            {
                onTriggerEnterChain.Handle(ball, trigger);
            }
        };

        handleTriggerExit += (ball, trigger) =>
        {
            onTriggerExitChain.Handle(ball, trigger);
        };
    }

    public void OnGameplayStarted()
    {
        Ball.onBallEnteredTrigger += handleTriggerEnter;
        Ball.onBallExitedTrigger += handleTriggerExit;

        multipliers = Object.FindObjectsOfType<Multiplier>();
        locks = Object.FindObjectsOfType<Lock>();
        windZones = Object.FindObjectsOfType<LocalWindZone>();
        portals = Object.FindObjectsOfType<Portal>();

        key = Object.FindObjectOfType<Key>();

        foreach (var multiplier in multipliers)
        {
            ballSystem.AddBallSpawner(multiplier);
        }

        foreach (var @lock in locks)
        {
            ballSystem.AddBallSpawner(@lock);
            ballSystem.AddBallDestroyer(@lock);
        }

        onTriggerEnterChain = 
            new OnTriggerEnterHandler(multipliers,
            new OnTriggerEnterHandler(locks,
            new OnTriggerEnterHandler(windZones,
            new OnTriggerEnterHandler(portals, null))));

        onTriggerExitChain =
            new OnTriggerExitHandler(windZones, null);
    }

    public void OnGameplayStopped()
    {
        Ball.onBallEnteredTrigger -= handleTriggerEnter;
        Ball.onBallExitedTrigger -= handleTriggerExit;

        if (multipliers != null)
        {
            foreach (var multiplier in multipliers)
            {
                ballSystem.RemoveBallSpawner(multiplier);
            }
        }

        if (locks != null)
        {
            foreach (var @lock in locks)
            {
                ballSystem.RemoveBallSpawner(@lock);
                ballSystem.RemoveBallDestroyer(@lock);
            }
        }
    }

    private bool TestMainTriggers(Ball ball, Collider trigger)
    {
        if (trigger == finishCollider)
        {
            onFinishTriggered?.Invoke();
            return true;
        }

        if (trigger == destroyCollider)
        {
            Object.Destroy(ball.gameObject);
            onBallDestroyed?.Invoke();
            return true;
        }

        if (key != null && trigger == key.Collider)
        {
            Object.Destroy(key.gameObject);
            onKeyCollected?.Invoke();
            return true;
        }

        return false;
    }
}
