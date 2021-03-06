using UnityEngine;
using System;
using System.Collections;
using live2d;
using live2d.framework;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SimpleModel : MonoBehaviour
{
    public TextAsset mocFile;
    public TextAsset physicsFile;
    public Texture2D[] textureFiles;
    public TextAsset[] mtnFiles;

    private Live2DModelUnity live2DModel;
    private EyeBlinkMotion eyeBlink = new EyeBlinkMotion();
    private L2DTargetPoint dragMgr = new L2DTargetPoint();
    private L2DPhysics physics;
    private Matrix4x4 live2DCanvasPos;
    private Live2DMotion motion;
    private MotionQueueManager motionManager;

    private State state = State.None;

    enum State
    {
        None,
        Watch,
        Smile
    }


    void Start()
    {
        Live2D.init();
        load();
    }


    void load()
    {
        live2DModel = Live2DModelUnity.loadModel(mocFile.bytes);

        for (int i = 0; i < textureFiles.Length; i++)
        {
            live2DModel.setTexture(i, textureFiles[i]);
        }

        float modelWidth = live2DModel.getCanvasWidth();
        live2DCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50.0f, 50.0f);

        if (physicsFile != null) physics = L2DPhysics.load(physicsFile.bytes);

        motion = Live2DMotion.loadMotion(mtnFiles[0].bytes);
        motion.setLoop(false);

        motionManager = new MotionQueueManager();
    }

    public void smile()
    {
        Debug.Log("smile");

        motionManager.startMotion(motion, true);
        state = State.Smile;
    }

    void Update()
    {
        if (live2DModel == null) load();
        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvasPos);
        if (!Application.isPlaying)
        {
            live2DModel.update();
            return;
        }

        var pos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            //
        }
        else if (Input.GetMouseButton(0))
        {
            var modelPos = Camera.main.WorldToScreenPoint(this.transform.position);
            var diffX = pos.x - modelPos.x + Screen.width / 2;
            var diffY = pos.y - modelPos.y + Screen.height / 2;
            dragMgr.Set(diffX / Screen.width * 2 - 1, diffY / Screen.height * 2 - 1);

            if (state == State.Watch)
            {
                state = State.None;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dragMgr.Set(0, 0);
        }
        else
        {
            if (UnityEngine.Random.Range(0, 300) == 0)
            {
                if (state == State.None)
                {
                    state = State.Watch;
                }
                else if (state == State.Watch)
                {
                    state = State.None;
                    dragMgr.Set(0, 0);
                }
                Debug.Log("state: " + state);
            }
        }

        if (state == State.Watch)
        {
            double timeSec = UtSystem.getUserTimeMSec() / 1000.0;
            double t = timeSec * 2 * Math.PI;
            dragMgr.Set((float)Math.Sin(t / 3.0), 0);
        }

        dragMgr.update();
        if (state == State.None || state == State.Watch)
        {
            live2DModel.setParamFloat("PARAM_ANGLE_X", dragMgr.getX() * 30);
            live2DModel.setParamFloat("PARAM_ANGLE_Y", dragMgr.getY() * 30);

            live2DModel.setParamFloat("PARAM_BODY_ANGLE_X", dragMgr.getX() * 10);

            live2DModel.setParamFloat("PARAM_EYE_BALL_X", dragMgr.getX());
            live2DModel.setParamFloat("PARAM_EYE_BALL_Y", dragMgr.getY());

            double timeSec = UtSystem.getUserTimeMSec() / 1000.0;
            double t = timeSec * 2 * Math.PI;
            live2DModel.setParamFloat("PARAM_BREATH", (float)(0.5f + 0.5f * Math.Sin(t / 3.0)));

            eyeBlink.setParam(live2DModel);
        }
        else if (state == State.Smile)
        {
            motionManager.updateParam(live2DModel);
            if (motionManager.isFinished())
            {
                state = State.None;
            }
        }

        if (physics != null) physics.updateParam(live2DModel);

        live2DModel.update();
    }


    void OnRenderObject()
    {
        if (live2DModel == null) load();
        if (live2DModel.getRenderMode() == Live2D.L2D_RENDER_DRAW_MESH_NOW) live2DModel.draw();
    }
}