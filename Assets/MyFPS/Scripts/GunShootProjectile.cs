using System;
using UnityEngine;

namespace MyFPS
{
    public class GunShootProjectile : MonoBehaviour
    {
        [Header("애니메이터 설정")]
        [SerializeField] private Animator gunAnimator;
        [SerializeField] private string shootTriggerName = "ShootTrg";
        [SerializeField] private string ammoParamName = "Ammo";

        [Header("사운드 설정")]
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private AudioSource audioSource;

        [Header("총구 화염 이펙트")]
        [SerializeField] private ParticleSystem muzzleFlash;

        [Header("탄환 발사 설정 (★수정/추가)")]
        [SerializeField] private GameObject bulletPrefab;               // 발사할 탄환 프리팹
        [SerializeField] private Transform firePoint;                   // 발사 기준점 (총구)
        [SerializeField] private float bulletSpeed = 50f;               // 탄환 속도
        [SerializeField] private int damage = 5;                        // 대미지
        [SerializeField] private float maxRange = 100f;                 // 조준점을 찾기 위한 최대 거리

        [Header("피격 이펙트 (★프리팹 형태로 사용)")]
        [SerializeField] private ParticleSystem hitParticle;            // 피격 시 생성할 파티클 프리팹

        [Header("장탄 수")]
        public int maxAmmo = 7;
        public int ammoCount = 0;

        private void OnEnable()
        {
            UpdateAnimatorAmmo();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        public void UpdateAnimatorAmmo()
        {
            if (gunAnimator != null)
            {
                gunAnimator.SetInteger(ammoParamName, ammoCount);
            }
        }

        private void Shoot()
        {
            if (ammoCount <= 0)
            {
                Debug.Log("<color=yellow>총알이 없습니다!</color>");
                return;
            }

            ammoCount--;
            UpdateAnimatorAmmo();

            if (gunAnimator != null)
            {
                gunAnimator.ResetTrigger(shootTriggerName);
                gunAnimator.SetTrigger(shootTriggerName);
            }

            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            if (muzzleFlash != null)
            {
                muzzleFlash.Stop();
                muzzleFlash.Play();
            }

            // ★ 물리 탄환 발사 로직 호출
            SpawnBullet();
        }

        private void SpawnBullet()
        {
            if (bulletPrefab == null || firePoint == null)
            {
                Debug.LogWarning("Bullet Prefab 또는 FirePoint가 지정되지 않았습니다.");
                return;
            }

            // 1. 화면 중앙(크로스헤어)이 바라보는 목표 지점(Target Point) 계산
            Camera mainCam = Camera.main;
            Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // 화면 중앙에서 정면으로 레이 생성
            RaycastHit hit;
            Vector3 targetPoint;

            // 정면에 무언가 조준되고 있다면 그 위치를 목표로 설정
            if (Physics.Raycast(ray, out hit, maxRange))
            {
                targetPoint = hit.point;
            }
            else
            {
                // 공중이나 먼 하늘을 보고 있다면 레이의 끝 지점을 목표로 설정
                targetPoint = ray.GetPoint(maxRange);
            }

            // 2. 총구(FirePoint)에서 목표 지점을 향하는 방향 벡터 계산
            Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

            // 3. 총구 위치에 탄환 오브젝트 생성
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // 4. 생성된 탄환의 Bullet 스크립트에 속도, 대미지, 파티클, 방향 전달하여 발사
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Setup(bulletSpeed, damage, hitParticle, shootDirection);
            }
        }
    }
}
