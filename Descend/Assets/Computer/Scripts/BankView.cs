using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BankView : MonoBehaviour
{
    Text mName;
    Text mCash;
    Text mDebt;
    Text mBalance;
    Text mMessage;
    Text mMinimumPayment;

    InputField mPaymentField;
    Button mPaymentButton;

    // Start is called before the first frame update
    void Start()
    {
        mName = transform.Find("Name").GetComponent<Text>();
        mCash = transform.Find("Cash").GetComponent<Text>();
        mDebt = transform.Find("Debt").GetComponent<Text>();
        mBalance = transform.Find("Balance").GetComponent<Text>();
        mMessage = transform.Find("Message").GetComponent<Text>();
        mMinimumPayment = transform.Find("MinPayment").GetComponent<Text>();

        mPaymentField = transform.Find("Payment/InputField").GetComponent<InputField>();
        mPaymentButton = transform.Find("PaymentButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        SaveData saveData = SaveData.Get();

        int cash = saveData.GetCash();
        int debt = saveData.debt;
        mCash.text = saveData.GetCash().ToString();
        mDebt.text = saveData.debt.ToString();
        mBalance.text = (debt - cash).ToString(); // UI always has a negative sign

        if(!saveData.madePaymentToday)
        {
            mPaymentField.readOnly = false;
            mPaymentButton.interactable = true;
            // hack to only reset stuff once
            if(mMessage.text.Length > 0)
            {
                mMessage.text = "";
                mPaymentField.text = saveData.GetMinimumPayment().ToString();
            }
            
            mName.text = GameConfig.playerName;            
            mMinimumPayment.text = saveData.GetMinimumPayment().ToString();
        }
    }

    public void OnEndEdit()
    {
        if(mPaymentField.readOnly) return;

        mPaymentButton.interactable = false;
        string text = mPaymentField.text;

        int paymentAmount = 0;
        if(text.Length > 0)
        {
            paymentAmount = int.Parse(text);
        }

        SaveData saveData = SaveData.Get();
        int cash = saveData.GetCash();
        int minimumPayment = saveData.GetMinimumPayment();
        if(paymentAmount > cash)
        {
            mPaymentField.text = cash.ToString();
        }
        else if(paymentAmount < minimumPayment && cash >= minimumPayment)
        {
            mPaymentField.text = minimumPayment.ToString();
        }
        
        if(paymentAmount >= minimumPayment)
        {
            mPaymentButton.interactable = true;
        }
    }

    public void MakeDebtPayment()
    {
        SaveData saveData = SaveData.Get();

        saveData.madePaymentToday = true;
        mPaymentField.readOnly = true;
        mPaymentButton.interactable = false;

        int paymentAmount = int.Parse(mPaymentField.text);
        saveData.DecreaseCash(paymentAmount);
        saveData.debt -= paymentAmount;

        mMessage.text = "Thank you! Payment received.";
    }
}
