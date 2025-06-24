using System;
using System.Collections;
using System.Collections.Generic;
using abc;
using LDtkUnity;
using Unity.VisualScripting;
using UnityEngine;

public class Read2 : MonoBehaviour
{
    public LDtkFields lDtkFields;
    public TriggerCondition triggerCondition;

    public GameObject repeater;
    // Start is called before the first frame update
    void Start()
    {
        lDtkFields = GetComponent<LDtkFields>();
        triggerCondition = lDtkFields.GetEnum<TriggerCondition>("condition");
        LDtkReferenceToAnEntityInstance[] refs=lDtkFields.GetEntityReferenceArray("onTrigger");
        // foreach (var entityRef in refs) {
        //     if (entityRef != null) {
        //         Object unityObj = entityRef.GetEntity();
        //         GameObject go = unityObj.GameObject();
        //
        //         if (go != null) {
        //             Debug.Log("引用目标实体: " + go.name);
        //         }
        //         else {
        //             Debug.LogWarning("引用对象不是 GameObject，可能是丢失的引用。");
        //         }
        //     }
        // }
        var actions = new Dictionary<string, Action<GameObject, LDtkFields>>
        {
            {
                "Repeater", (go, fields) =>
                {
                    Debug.Log("这是 Repeater");

                    if (fields != null) {
                        var a=fields.GetEntityReferenceArray("targets");
                        var b = new Dictionary<string, Action<GameObject, LDtkFields>>
                        {
                            {
                                "SpotLight", (go, fields) =>
                                {
                                    if (fields != null) {
                                        Debug.Log("这是 SpotLight");
                                    }
                                }
                            
                            }
                        };
                        // var count = fields.GetInt("repeatCount");
                        // Debug.Log("内部字段 repeatCount = " + count);
                        LDtkRefUtils.HandleEntityReferencesWithFields(a, b);
                    }
                    
                }
            },
            {
                "MessagePopUp", (go, fields) =>
                {
                    Debug.Log("这是 MessagePopUp");

                    // if (fields != null && fields.TryGetString("message", out string msg)) {
                    //     Debug.Log("显示消息：" + msg);
                    // }
                }
            }
        };
        LDtkRefUtils.HandleEntityReferencesWithFields(refs, actions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public void OnLDtkImportFields(LDtkFields fields){
    //     triggerCondition=fields.GetEnum<TriggerCondition>("condition");
    //     
    // }
}

public static class LDtkRefUtils
{
    public static void HandleEntityReferencesWithFields(
        LDtkReferenceToAnEntityInstance[] refs,
        Dictionary<string, Action<GameObject, LDtkFields>> actionsByName){
        if (refs == null || refs.Length == 0) {
            Debug.LogWarning("引用为空");
            return;
        }

        for (int i = 0; i < refs.Length; i++) {
            var iid = refs[i].GetEntity();
            if (iid == null) {
                Debug.LogWarning($"第 {i} 个引用目标未激活或未找到");
                continue;
            }

            GameObject go = iid.gameObject;
            LDtkFields fields = go.GetComponent<LDtkFields>();

            Debug.Log($"第 {i} 个引用对象名: {go.name}");

            bool matched = false;
            foreach (var kv in actionsByName) {
                if (go.name.Contains(kv.Key)) {
                    kv.Value?.Invoke(go, fields);
                    matched = true;
                    break;
                }
            }

            if (!matched) {
                Debug.Log("未匹配到关键词，跳过: " + go.name);
            }
        }
    }

    public static void HandleEntityReferences(
        LDtkReferenceToAnEntityInstance[] refs,
        Dictionary<string, Action<GameObject>> actionsByName){
        if (refs == null || refs.Length == 0) {
            Debug.LogWarning("引用为空");
            return;
        }

        for (int i = 0; i < refs.Length; i++) {
            var iid = refs[i].GetEntity();
            if (iid == null) {
                Debug.LogWarning($"第 {i} 个引用目标未激活或未找到");
                continue;
            }

            GameObject go = iid.gameObject;
            Debug.Log($"第 {i} 个引用对象名: {go.name}");

            bool matched = false;
            foreach (var kv in actionsByName) {
                if (go.name.Contains(kv.Key)) {
                    kv.Value?.Invoke(go);
                    matched = true;
                    break;
                }
            }

            if (!matched) {
                Debug.Log("未匹配到关键词，跳过: " + go.name);
            }
        }
    }

}