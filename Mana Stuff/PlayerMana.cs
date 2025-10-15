using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour, ISaveManager
{
    public static PlayerMana instance { get; private set; }

    private float m_Mana;
    [SerializeField] private float m_ManaMax;
    [SerializeField] private float m_ManaRechargeRate;
    private int m_ManaUpgradeProgress;

    [SerializeField] private Slider manaSlider;
    private Coroutine activelyChargingMana;

    public void LoadData(GameData data)
    {
        m_ManaUpgradeProgress = data.manaUpgradeFragmentCount;
        m_ManaMax = data.maxPlayerMana;
        manaSlider.GetComponent<RectTransform>().sizeDelta = data.manaSliderSize;
    }

    public void SaveData(GameData data)
    {
        data.manaUpgradeFragmentCount = m_ManaUpgradeProgress;
        data.maxPlayerMana = m_ManaMax;
        data.manaSliderSize = manaSlider.GetComponent<RectTransform>().sizeDelta;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            instance = this;
    }

    private void Start()
    {
        m_Mana = m_ManaMax;

        manaSlider.maxValue = m_ManaMax;
        manaSlider.value = m_Mana;
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {
            CollectManaFragment();
            Debug.Log("Collected mana fragment, current shards: " + m_ManaUpgradeProgress);
        }
    }

    public bool SpendMana(float amount)
    {
        bool canSpend = false;

        if(m_Mana - amount >= 0)
        {
            m_Mana -= amount;
            canSpend = true;
        }
        //TO DO: CANT SPEND MANA WARNING.
        UpdateUI();
        if (activelyChargingMana == null)
            activelyChargingMana = StartCoroutine(RechargeMana());

        return canSpend;
    }

    public void CollectManaFragment()
    {
        m_ManaUpgradeProgress++;
        if (m_ManaUpgradeProgress == 4)
        {
            m_ManaMax += m_ManaMax / 5;
            manaSlider.maxValue = m_ManaMax;
            RectTransform sliderSize = manaSlider.GetComponent<RectTransform>();
            sliderSize.sizeDelta = new Vector2(sliderSize.sizeDelta.x + sliderSize.sizeDelta.x / 5, sliderSize.sizeDelta.y);
            m_ManaUpgradeProgress = 0;
        }
    }

    public void UpdateUI()
    {
        manaSlider.value = m_Mana;
    }

    private IEnumerator RechargeMana()
    {
        float time = 0;

        while (m_Mana < m_ManaMax)
        {
            time += Time.deltaTime;
            yield return null;
            if(time >= 5f)
            {
               m_Mana += m_ManaRechargeRate;

                UpdateUI();
                time = 0;
            }
        }
        m_Mana = m_ManaMax;
        StopCoroutine(activelyChargingMana);
        activelyChargingMana = null;
    }
}
