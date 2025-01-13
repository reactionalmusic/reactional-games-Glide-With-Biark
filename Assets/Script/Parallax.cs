using Reactional.Playback;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Parallax Scroll Settings")]
    [SerializeField] private float parallaxScrollSpeed;
    
    [Header("Parallax Pulse Settings")]
    [SerializeField] private bool extraspeed = false;
    [SerializeField] private AnimationCurve parallaxCurve;
    
    [Header("Reactional Quant Settings")]
    [SerializeField] private float quant = 2;
    private float speedup;

    private float start_pos;
    private Bounds group_bounds;

    void Start() {
        group_bounds = get_bounds(gameObject);
        start_pos = transform.position.x;
    }
   
    private void Update() {
        if (extraspeed) {
            speedup = parallaxCurve.Evaluate(MusicSystem.GetCurrentBeat() % quant);
        } else {
            speedup = 0;
        }

        var t = transform;
        var dist = t.position.x - (parallaxScrollSpeed + speedup) * Time.deltaTime;

        t.position = new Vector3(dist, t.position.y, t.position.z);

        if (t.position.x < start_pos - group_bounds.extents.x) {
            t.position = new Vector3(start_pos, t.position.y, t.position.z);
        }
    }

    private static Bounds get_bounds(GameObject game_object) {
        Bounds bounds = new  Bounds(Vector3.zero,Vector3.zero);
        
        Renderer render = game_object.GetComponent<Renderer>();
        if (render !=null) { bounds = render.bounds; }
        
        if (bounds.extents.x == 0) {
            bounds = new Bounds(game_object.transform.position,Vector3.zero);
            foreach (Transform child in game_object.transform) {
                var child_render = child.GetComponent<Renderer>();
                bounds.Encapsulate(
                    (child_render != null)
                        ? child_render.bounds
                        : get_bounds(child.gameObject)
                );
            }
        }
        return bounds;
    }
}
