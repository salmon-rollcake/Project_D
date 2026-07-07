using System;
using Unity.VisualScripting;
using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 총 발사 상호작용을 제어하는 클래스
    /// 마우스 왼쪽 클릭 → 발사 애니메이션 + 사운드 + 총구 화염 이펙트 재생
    /// 레이캐스트로 적 감지 → 대미지 적용 + 피격 파티클 재생
    /// </summary>
    public class GunShoot : MonoBehaviour
    {
        [Header("애니메이터 설정")]
        [SerializeField] private Animator gunAnimator;                  // 총 오브젝트의 Animator
        [SerializeField] private string shootTriggerName = "ShootTrg"; // 발사 Trigger 파라미터 이름
        [SerializeField] private string ammoParamName = "Ammo";        // 애니메이터의 Ammo 파라미터 이름

        [Header("사운드 설정")]
        [SerializeField] private AudioClip shootSound;                  // 발사 효과음
        [SerializeField] private AudioSource audioSource;               // 재생할 오디오 소스

        [Header("총구 화염 이펙트")]
        [SerializeField] private ParticleSystem muzzleFlash;            // 총구에 붙여둔 파티클 시스템

        [Header("레이캐스트 설정")]
        [SerializeField] private float range = 5f;                      // 사거리
        [SerializeField] private int damage = 5;                        // 대미지
        [SerializeField] private Transform firePoint;                   // 레이 발사 기준점 (없으면 카메라 기준)

        [Header("피격 이펙트")]
        [SerializeField] private ParticleSystem hitParticle;            // 피격 시 재생할 파티클

        [Header("장탄 수")]
        public int maxAmmo = 7;                       // 최대 장탄 수
        public int ammoCount = 0;                     // 장탄 수

        private void OnEnable()
        {
            UpdateAnimatorAmmo();
        }

        void Update()
        {
            // 마우스 왼쪽 버튼 클릭 감지
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        // 애니메이터의 Ammo 파라미터를 갱신하는 헬퍼 메서드 (★추가)
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

            ammoCount--; // 발사 시 장탄 수 감소
            UpdateAnimatorAmmo();

            {
                // 1. 발사 애니메이션 Trigger 활성화
                if (gunAnimator != null)
                {
                    gunAnimator.ResetTrigger(shootTriggerName);
                    gunAnimator.SetTrigger(shootTriggerName);
                }

                // 2. 발사 사운드 재생
                if (audioSource != null && audioSource.isActiveAndEnabled && shootSound != null)
                {
                    audioSource.PlayOneShot(shootSound);
                }

                // 3. 총구 화염 이펙트 재생
                if (muzzleFlash != null)
                {
                    muzzleFlash.Stop();    // 이전 파티클이 남아있을 경우 초기화
                    muzzleFlash.Play();
                }

                // 4. 레이캐스트로 적 감지 및 대미지 적용
                FireRaycast();
            }
        }

        private void FireRaycast()
        {
            // firePoint가 없으면 메인 카메라 기준으로 발사
            Transform origin = (firePoint != null) ? firePoint : Camera.main.transform;

            RaycastHit hit;
            if (Physics.Raycast(origin.position, origin.forward, out hit, range))
            {
                // 디버그용: 씬 뷰에서 레이 확인 (초록 = 충돌, 빨강 = 미충돌)
                Debug.DrawRay(origin.position, origin.forward * hit.distance, Color.green, 0.5f);

                // 피격 파티클 재생 (충돌 지점에서 재생)
                if (hitParticle != null)
                {
                    hitParticle.transform.position = hit.point;
                    hitParticle.transform.rotation = Quaternion.LookRotation(hit.normal); // 표면 법선 방향으로 회전
                    hitParticle.Stop();
                    hitParticle.Play();
                }

                // 대미지를 받을 수 있는 오브젝트에 대미지 적용 (IDamageable 인터페이스)
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                    Debug.Log($"<color=cyan>'{hit.collider.name}'에 {damage}의 대미지를 입혔습니다.</color>");
                }
            }
            else
            {
                // 아무것도 맞지 않은 경우 레이 시각화
                Debug.DrawRay(origin.position, origin.forward * range, Color.red, 0.5f);
            }
        }
    }
}

