using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameCurrency
{
    Coins = 0
}

public class GameCurrencySystem
{
    public class CurrencyValue
    {
        private bool isPersistent;
        private GameCurrency currency;
        private int unprotectedIntValue;
        private string persistentStringValue;
        private System.Action<GameCurrency, int> onSetValue;
        public int Value
        {
            get
            {
                if (isPersistent)
                {
                    return IntCoder.Decode(GetType(), (int)currency, persistentStringValue);
                }
                else
                {
                    return unprotectedIntValue;
                }
            }
            set
            {
                if (isPersistent)
                {
                    persistentStringValue = IntCoder.Encode(GetType(), (int)currency, value);
                }
                else
                {
                    unprotectedIntValue = value;
                }
                onSetValue?.Invoke(currency, value);
            }
        }

        public CurrencyValue(bool isPersistent, GameCurrency currency, System.Action<GameCurrency, int> onSetValue)
        {
            this.isPersistent = isPersistent;
            this.currency = currency;
            this.onSetValue = onSetValue;
        }
    }

    private Dictionary<GameCurrency, CurrencyValue> map;
    private Dictionary<GameCurrency, CurrencyValue> buffers;

    public GameCurrencySystem(UserInterfaceSystem ui)
    {       
        map = new Dictionary<GameCurrency, CurrencyValue>();
        buffers = new Dictionary<GameCurrency, CurrencyValue>();

        var currencies = (GameCurrency[])System.Enum.GetValues(typeof(GameCurrency));

        System.Action<GameCurrency, int> onSetMainCurrencyValueCallback = null;
        onSetMainCurrencyValueCallback += (currency, value) => PersistentData.SetIntEncoded(GetType(), (int)currency, value);
        onSetMainCurrencyValueCallback += ui.DisplayCurrency;

        System.Action<GameCurrency, int> onSetBufferCurrencyValueCallback = ui.DisplayBufferCurrency;

        foreach (var currency in currencies)
        {
            var persistentCurrencyValue = new CurrencyValue(true, currency, onSetMainCurrencyValueCallback);
            persistentCurrencyValue.Value = PersistentData.GetIntDecoded(GetType(), (int)currency);
            map.Add(currency, persistentCurrencyValue);

            var bufferCurrencyValue = new CurrencyValue(true, currency, onSetBufferCurrencyValueCallback);
            buffers.Add(currency, bufferCurrencyValue);
        }
    }

    public void SetBuffer(GameCurrency currency, int amount)
    {
        buffers[currency].Value = amount;
    }

    public void MultiplyBuffer(GameCurrency currency, int multiplier)
    {
        buffers[currency].Value *= multiplier;
    }

    public void AddFromBuffer(GameCurrency currency)
    {
        map[currency].Value += buffers[currency].Value;
    }

    public void Add(GameCurrency currency, int amount)
    {
        map[currency].Value += amount;
    }

    public void Spend(GameCurrency currency, int amount)
    {
        map[currency].Value -= amount;
    }

    public bool IsEnough(GameCurrency currency, int price)
    {
        return map[currency].Value >= price;
    }
}
