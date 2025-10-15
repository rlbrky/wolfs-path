using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, ISaveManager
{
    public static PlayerHealth instance { get; private set; }

    public float m_HealthMax;
    private float m_Health;
    private int m_HealthUpgradeProgress;
    private Vector2 sliderSize;

    [SerializeField] private Slider m_HealthSlider;
    [SerializeField] private DeathUI m_DeathUI;

    public void LoadData(GameData data)
    {
        m_HealthMax = data.maxPlayerHealth;
        m_HealthUpgradeProgress = data.healthUpgradeFragmentCount;
        sliderSize = data.healthSliderSize;
    }

    public void SaveData(GameData data)
    {
        data.healthUpgradeFragmentCount = m_HealthUpgradeProgress;
        data.maxPlayerHealth = m_HealthMax;
        data.healthSliderSize = sliderSize;
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

        m_DeathUI = FindObjectOfType<DeathUI>();
        m_DeathUI.gameObject.SetActive(false);
    }

    private void Start()
    {
        m_HealthSlider.GetComponent<RectTransform>().sizeDelta = sliderSize;
        m_Health = m_HealthMax;

        m_HealthSlider.maxValue = m_HealthMax;
        m_HealthSlider.value = m_Health;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.J))
        {
            CollectHealthFragment();
            Debug.Log("Collected health fragment, current shards: " + m_HealthUpgradeProgress);
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            DamagePlayer(100);
        }
    }

    public void DamagePlayer(float damage)
    {
        m_Health -= damage;
        UpdateUI();
        if (m_Health <= 0)
        {
            //Player Death Actions
            Time.timeScale = 0f;
            m_DeathUI.gameObject.SetActive(true);
            m_DeathUI.ActivateUI();

            IBossEvents[] bosses = FindObjectsOfType<MonoBehaviour>().OfType<IBossEvents>().ToArray();
            if (bosses.Length > 0) 
            {
                foreach (var _boss in bosses)
                    _boss.ResetBoss();
            }

            IEnemy[] enemies = FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>().ToArray();
            if(enemies.Length > 0)
            {
                foreach (var enemy in enemies)
                    enemy.Revive();
            }
            //gameObject.SetActive(false);
        }
    }

    public void Heal(float amount)
    {
        if(m_Health < m_HealthMax)
        {
            m_Health += amount;

            if(m_Health > m_HealthMax)
                m_Health = m_HealthMax;
        }
        UpdateUI();
    }

    public void FullHeal()
    {
        m_Health = m_HealthMax;
        UpdateUI();
    }

    public void CollectHealthFragment()
    {
        m_HealthUpgradeProgress++;
        if (m_HealthUpgradeProgress == 4)
        {
            m_HealthMax += m_HealthMax / 5;
            m_HealthSlider.maxValue = m_HealthMax;
            RectTransform sliderTransform = m_HealthSlider.GetComponent<RectTransform>();
            sliderSize = new Vector2(sliderTransform.sizeDelta.x + sliderTransform.sizeDelta.x / 5, sliderTransform.sizeDelta.y);
            m_HealthUpgradeProgress = 0;
            sliderTransform.sizeDelta = sliderSize;
        }
        DataManager.Instance.SaveGame();
    }

    private void UpdateUI()
    {
        m_HealthSlider.value = m_Health;
    }
}
