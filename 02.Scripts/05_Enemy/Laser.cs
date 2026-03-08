using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;


public class Laser : MonoBehaviour
{
    public GameObject laser;
    private SpriteRenderer sr;
    private BoxCollider2D bc;

    int layerMask;
    public float damageRatio = 5f;
    public EnemyController enemyController;

    float tickDelay = 1.5f;
    float DelayTime;

    public float dIstance;

    private void Awake()
    {
        DelayTime = Time.time;
        GetReferences();
        //InitFields();
        layerMask = LayerMask.GetMask("Obstacle", "Ship");
        SoundManager.Instance.StartRepeatingSFX("RepeatingBeamSound", transform.position, 1f);
    }

    private void OnDisable()
    {
        SoundManager.Instance.StopRepeatingSFX("RepeatingBeamSound");
    }
    private void GetReferences()
    {
        sr = GetComponent<SpriteRenderer>();
        //bc = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        FormLaser();
    }

    private void FormLaser()
    {
        SoundManager.Instance.PlaySFX("LaserBeam", transform.position, 1f);
        float defaultDistance = 30f;
        float currentDistance = defaultDistance;
        
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.up, defaultDistance, layerMask);
        
        if (ray.collider != null)
        {
            currentDistance = ray.distance;
            if ((layerMask & (1 << ray.collider.gameObject.layer)) != 0)
            {
                if (DelayTime < Time.time)
                {

                    var temp = ray.collider.gameObject.GetComponent<IDamageable>();
                    if(temp != null)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            temp.OnDamage(enemyController.enemyStat.attackPower * damageRatio);
                            DelayTime = Time.time + tickDelay;
                        }
                    }
                    
                }

            }
        }
        Debug.DrawRay(transform.position, transform.up * currentDistance, Color.red);
        dIstance = currentDistance ;
        float scaleValue = currentDistance / 3f;
        laser.transform.localScale = new Vector3(10f, scaleValue - 0.3f,1f);
        laser.transform.localPosition = new Vector3(laser.transform.localPosition.x,(scaleValue / 2f), 1f);
        //sr.size = new Vector2(currentDistance, 0.75f);
        //bc.size = new Vector2(currentDistance, 0.5f);
        //bc.offset = new Vector2(currentDistance / 2f, 0);
    }
}