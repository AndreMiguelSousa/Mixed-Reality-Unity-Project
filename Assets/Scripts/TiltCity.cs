using UnityEngine;

public class TiltCity : MonoBehaviour
{
        public Transform cityRoot;

        public void Tilt()
        {
            LeanTween.rotateZ(cityRoot.gameObject, 10f, 1f).setLoopPingPong(2);
        }
}
