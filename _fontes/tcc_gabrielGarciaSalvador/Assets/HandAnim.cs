using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.Oculus;
using OVR;
using TMPro; // Add the TextMesh Pro namespace to access the various functions.
using System.Linq;

public class HandAnim : MonoBehaviour
{
  public XRController controller = null;
  public Animator m_animator = null;

  public const string ANIM_LAYER_NAME_POINT = "Point Layer";
  public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";
  public const string ANIM_PARAM_NAME_FLEX = "Flex";
  public const string ANIM_PARAM_NAME_POSE = "Pose";

  private int m_animLayerIndexThumb = -1;
  private int m_animLayerIndexPoint = -1;
  private int m_animParamIndexFlex = -1;
  private int m_animParamIndexPose = -1;
  private Collider[] m_colliders = null;

  public float anim_frames = 4f;
  private float grip_state = 0f;
  private float trigger_state = 0f;
  private float triggerCap_state = 0f;
  private float thumbCap_state = 0f;

    private void Awake(){
      // animator =
      // controller.GetComponent<XRController>();
    }
    // Start is called before the first frame update
    void Start()
    {
      m_colliders = this.GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
      for (int i = 0; i < m_colliders.Length; ++i)
      {
          Collider collider = m_colliders[i];
          // collider.transform.localScale = new Vector3(COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN);
          collider.enabled = true;
      }
      m_animLayerIndexPoint = m_animator.GetLayerIndex(ANIM_LAYER_NAME_POINT);
      m_animLayerIndexThumb = m_animator.GetLayerIndex(ANIM_LAYER_NAME_THUMB);
      m_animParamIndexFlex = Animator.StringToHash(ANIM_PARAM_NAME_FLEX);
      m_animParamIndexPose = Animator.StringToHash(ANIM_PARAM_NAME_POSE);

      // var inputDevices = new List<UnityEngine.XR.InputDevice>();
      // UnityEngine.XR.InputDevices.GetDevices(inputDevices);
      //
      // foreach (var device in inputDevices)
      // {
      //     Debug.Log(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.role.ToString()));
      //     solutionText.SetText(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.role.ToString()));
      //
      // }
      }

    // Update is called once per frame
    void Update()
    {
      if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripTarget)){
        // solutionText.SetText(gripValue.ToString());
        float grip_state_delta = gripTarget - grip_state;
        if (grip_state_delta > 0f){  grip_state = Mathf.Clamp(grip_state + 1/anim_frames, 0f, gripTarget);
        }else if (grip_state_delta < 0f){grip_state = Mathf.Clamp(grip_state - 1/anim_frames, gripTarget, 1f);
        }else{  grip_state = gripTarget;}

        m_animator.SetFloat(m_animParamIndexFlex, grip_state);
      }
      if (controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerTarget)){

        float trigger_state_delta = triggerTarget - trigger_state;
        if (trigger_state_delta > 0f){  trigger_state = Mathf.Clamp(trigger_state + 1/anim_frames, 0f, triggerTarget);
        }else if (trigger_state_delta < 0f){trigger_state = Mathf.Clamp(trigger_state - 1/anim_frames, triggerTarget, 1f);
        }else{  trigger_state = triggerTarget;}

        m_animator.SetFloat("Pinch", trigger_state);
      }

        if (controller.inputDevice.TryGetFeatureValue(OculusUsages.indexTouch, out bool indexTouching))
        {
            if (indexTouching)
            {
                triggerCap_state = 0f;
            }
            else
            {
                triggerCap_state = 4f;
            }
            m_animator.SetLayerWeight(m_animLayerIndexPoint, triggerCap_state);
        }

        if (controller.inputDevice.TryGetFeatureValue(OculusUsages.thumbTouch, out bool thumbTouching))
        {

            m_animator.SetLayerWeight(m_animLayerIndexThumb, thumbTouching ? 0f : 4f);
        }
    }
}