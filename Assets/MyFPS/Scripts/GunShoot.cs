using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 총 발사 상호작용을 제어하는 클래스
    /// 마우스 왼쪽 클릭 → 발사 애니메이션 + 사운드 + 총구 화염 이펙트 재생
    /// </summary>
    public class GunShoot : MonoBehaviour
    {
        [Header("애니메이터 설정")]
        [SerializeField] private Animator gunAnimator;             // 총 오브젝트의 Animator
        [SerializeField] private string shootTriggerName = "ShootTrg"; // 발사 Trigger 파라미터 이름

        [Header("사운드 설정")]
        [SerializeField] private AudioClip shootSound;             // 발사 효과음
        [SerializeField] private AudioSource audioSource;          // 재생할 오디오 소스

        [Header("총구 화염 이펙트")]
        [SerializeField] private ParticleSystem muzzleFlash;       // 총구에 붙여둔 파티클 시스템

        void Update()
        {
            // 마우스 왼쪽 버튼 클릭 감지
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            // 1. 발사 애니메이션 Trigger 활성화
            if (gunAnimator != null)
            {
                gunAnimator.SetTrigger(shootTriggerName);
            }

            // 2. 발사 사운드 재생
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            // 3. 총구 화염 이펙트 재생
            if (muzzleFlash != null)
            {
                muzzleFlash.Stop();    // 이전 파티클이 남아있을 경우 초기화
                muzzleFlash.Play();
            }
        }
    }
}
