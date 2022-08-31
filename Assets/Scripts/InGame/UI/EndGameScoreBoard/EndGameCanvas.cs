using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.UI.EndGameCanvas
{
    public class EndGameCanvas : MonoBehaviour
    {
        [SerializeField]
        private GameObject PVPScoreBoardPrefab;
        [SerializeField]
        private GameObject countDownTimerPrefab;

        [SerializeField]
        private Transform background;

        public CountDownTimer countDownTimer { get; private set; }
 

        private void OnEnable()
        {
            Instantiate(PVPScoreBoardPrefab, background);
            StartCoroutine(displayTimer());
        }

        IEnumerator displayTimer()
        {
            // yield return new WaitForSeconds(PVPScoreBoard.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
            yield return new WaitForSeconds(1);
            countDownTimer = Instantiate(countDownTimerPrefab, background).GetComponent<CountDownTimer>();
        }
    }
}