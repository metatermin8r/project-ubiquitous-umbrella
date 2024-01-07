using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("UI References")]
    [SerializeField] Image healthbarImage;
    [SerializeField] Image staminabarImage;
    //[SerializeField] GameObject ui;
    //[SerializeField] PauseMenu pm;

    [Header("Stamina Stats")]
    const float maxStamina = 100f;
    public float currentStamina = maxStamina;
    float staminaCost = 10;
    public Coroutine staminaRegeneration;
    [Range(0.0f, 10.0f)]
    public float staminaRegen = 0.1f;
    public int staminaRegenDelay = 2;

    [Header("Health Stats")]
    const float maxHealth = 100f;
    public float currentHealth = maxHealth;
    public Coroutine healthRegeneration;
    [Range(0.0f, 10.0f)]
    public float healthRegen = 0.1f;
    public int healthRegenDelay = 2;

    // Start is called before the first frame update
    void Start()
    {
        healthbarImage = GameObject.Find("Player_Hud/HUD_Canvas/PlayerStats/HealthbarBackground/Healthbar").GetComponent<Image>();//.transform.GetChild(0).GetChild(2).GetChild(0).GetComponentInChildren<Image>
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TakeDamage(10);
        }
    }

    public void Sprint()
    {
        //if (playerAnimator != null)
        //playerAnimator.SetBool("Sprinting", true);

        //currentStamina -= staminaCost * Time.deltaTime;

        //staminabarImage.fillAmount = currentStamina / maxStamina;

        //if (staminaRegeneration != null)
            //StopCoroutine(staminaRegeneration);

        //staminaRegeneration = StartCoroutine(StaminaRegen());
    }

    //private IEnumerator StaminaRegen()
    //{
    //    yield return new WaitForSeconds(staminaRegenDelay);

    //    while (currentStamina < maxStamina)
    //    {
    //        currentStamina += maxStamina / 100;
    //        staminabarImage.fillAmount = currentStamina / maxStamina;
    //        yield return new WaitForSeconds(staminaRegen);
    //    }
    //    staminaRegeneration = null;
    //}

    public void TakeDamage(float damage)
    {
        RPC_TakeDamage(damage); //PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    //[PunRPC]
    void RPC_TakeDamage(float damage)//, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        //if (currentHealth <= 30)
            //healthbarImage.color = new Color32(240, 0, 0, 140);

        if (healthRegeneration != null)
            StopCoroutine(healthRegeneration);

        healthRegeneration = StartCoroutine(HealthRegen());

        if (currentHealth <= 0)
        {
            //Finds Player weapon item index to correctly assign image of the weapon that killed them in the KillFeed
            //int howIndex = PlayerManager.Find(info.Sender).weaponIndex;

            //Handles sending kill data to Player Manager for Scoreboard
            //PlayerManager.Find(info.Sender).GetKill();

            //Finds Player Nickname for KillFeed
            //string killer = PlayerManager.Find(info.Sender).GetComponent<PhotonView>().Owner.NickName.ToString();

            //Calls RPC for KillFeed and sends data
            //KillListWithImage(killer, PV.Owner.NickName, howIndex); //, info);

            //PV.RPC("RPC_KillListWithHow", RpcTarget.All, killer, PV.Owner.NickName, "killed"); 

            Die(); //Kills player as normal
        }
        Debug.Log("took damage: " + damage);
    }

    private IEnumerator HealthRegen()
    {
        yield return new WaitForSeconds(healthRegenDelay);

        while (currentHealth < maxHealth)
        {
            currentHealth += maxHealth / 100;
            healthbarImage.fillAmount = currentHealth / maxHealth;
            //if (currentHealth >= 31)
                //healthbarImage.color = new Color32(255, 255, 255, 140);
            yield return new WaitForSeconds(healthRegen);
        }
        healthRegeneration = null;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
