#if UNITY_EDITOR
using UnityEngine;

namespace Reactional.Experimental
{
    public class GenerateTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Reactional.Experimental.GenerateAsset.All();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
#endif