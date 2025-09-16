using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class BarLifeEnemy : NetworkBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private Enemy enemy;
    private int lifeEnemyMax;

    private Image imageLife;
    private void OnEnable()
    {
        enemy.OnLife += UpdateLife;
    }
    private void OnDisable()
    {
        enemy.OnLife -= UpdateLife;

    }
    private void Awake()
    {
        imageLife = GetComponent<Image>();
    }
    private void Start()
    {
        lifeEnemyMax = enemy.life;
    }
    private void UpdateLife(int live)
    {
        imageLife.fillAmount = (float)live / lifeEnemyMax;
    }
}
