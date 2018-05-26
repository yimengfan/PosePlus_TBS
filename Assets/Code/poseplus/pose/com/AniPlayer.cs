using System;
using System.Collections.Generic;
using BDFramework.ResourceMgr;
using UnityEngine;
using Game.ReourceMgr;
namespace FB.PosePlus
{
    //新的动画控制器，相比封闭的animator，建立一个更开放自由的模式
    public class AniPlayer : MonoBehaviour
    {
        public int tagid;
        void Start()
        {
            bPlayRunTime = true;        
        }

        public List<FB.PosePlus.AniClip> clips;

        Dictionary<string, int> clipcache = null;


        /// <summary>
        /// 当前播放动画
        /// </summary>
        public AniClip CurClip
        {
            get { return lastClip; }   
        }       
        /// <summary>
        /// 当前帧id
        /// </summary>
        public int CurAniFrame
        {
            get { return lastframe; }
            
        }
        bool bPlayRunTime = false;

        private AniClip lastClip;//当前剪辑
        int lastframe = -1; //当前帧

        /// <summary>
        /// 当前帧的总计数
        /// </summary>
        int frameCounter = -1;

       

        bool bLooped = false;
        int startframe;
        int endframe;
        float _crossTimer = -1;
        float _crossTimerTotal = 0;
        Frame crossFrame = null; //用来混合用的帧

        public Frame frameNow { get; private set; }

        #region 动画处理
        /// <summary>
        /// 播放一个动画片段 
        /// </summary>
        /// <param name="clip"></param>
        public void Play(string clip)
        {
           var c = this.GetClip(clip);
           this.Play(c);

        }
        
        
        /// <summary>
        /// 获取一个动画片段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AniClip GetClip(string name)
        {
            if (clips == null || clips.Count == 0) return null;
            if (clipcache == null || clipcache.Count != clips.Count)
            {
                clipcache = new Dictionary<string, int>();
                for (int i = 0; i < clips.Count; i++)
                {
                    // if (clipcache.ContainsKey(clips[i].name))
                    clipcache[clips[i].name] = i;
                }
            }

            int igot = 0;

            if (name.EndsWith(".FBAni") == false)
            {
                name += ".FBAni";
            }
            if (clipcache.TryGetValue(name, out igot))
            {
                return clips[igot];
            }

            return null;
        }

        /// <summary>
        /// 播放一个动画片段
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="clipsub"></param>
        /// <param name="crosstimer"></param>
        public void Play(AniClip clip,  SubClip clipsub = null,bool? clipLoop =null,  bool? clipsubLoop =null, float crosstimer = 0)
        {
            if (clipsub != null)
            {
                bLooped = clipsub.loop;
                startframe = (int) clipsub.startframe;
                endframe = (int) clipsub.endframe;
                if (_fps < 0)
                {
                    _fps = clip.fps;
                }
            }
            else if (clip != null)
            {
                if (clipLoop != null)
                {
                    bLooped = (bool) clipLoop;
                }
                else
                {
                    bLooped = clip.loop;
                }               
                startframe = 0;
                endframe = (clip.aniFrameCount - 1);
                if (_fps < 0)
                {
                    _fps = clip.fps;
                }
            }


          
            if (crosstimer <= 0)
            {
                this._crossTimer = -1;
                crossFrame = null;

                lastClip = clip;
                lastframe = startframe;
              
                SetPose(clip, startframe, true);
                frameNow = lastClip.frames[lastframe];
                // TODO 修复设置后立马推帧问题
                timer = frameCounter / _fps;
            }
            else
            {
                if (lastClip != null && lastframe >= 0 && lastframe < lastClip.frames.Count)
                {
                    RecCrossFrame();
                    lastClip = clip;
                    lastframe =startframe;
                    // TODO 修复设置后立马推帧问题
                    timer = frameCounter / _fps;
                }
                else
                {
                    lastClip = clip;
                    lastframe = startframe;
                    
                    SetPose(clip, startframe, true);
                    frameNow = lastClip.frames[lastframe];
                    // TODO 修复设置后立马推帧问题
                    timer = frameCounter / _fps;
                }

                this._crossTimerTotal = this._crossTimer = crosstimer;
            }
        }

        void RecCrossFrame()
        {
            if (this._crossTimer >= 0 && crossFrame != null)
            {
                Frame f = new Frame();

                float l = 1.0f - _crossTimer / _crossTimerTotal;
                crossFrame = Frame.Lerp(crossFrame, lastClip.frames[lastframe], l);
            }
            else
            {
                crossFrame = lastClip.frames[lastframe];
            }
        }
        #endregion
        float timer = 0;
        float _fps = -1;
        float pauseTimer = 0;

        public void _OnUpdate(float delta)
        {
            //帧推行
            if (lastClip == null)
                return;
            //打中暂停机制
//            if (pauseframe > 0)
//            {
//                pauseTimer += delta;
//                int pid = (int) ((timer + pauseTimer) * _fps);
//                if (pid - frameCounter >= pauseframe)
//                {
//                    pauseframe = 0;
//                    pauseTimer = 0;
//                }
//                else
//                {
//                    return;
//                }
//            }

            timer += delta;

            bool crossend = false;
            if (_crossTimer >= 0)
            {
                _crossTimer -= delta;
                if (_crossTimer <= 0)
                    crossend = true;
            }

            int _frameCount = (int) (timer * _fps); //这里要用一个稳定的fps，就用播放的第一个动画的fps作为稳定fps
            

            //
            if (_frameCount == frameCounter)
                return;

            if (_frameCount > frameCounter + 1) //增加一个限制，不准动画跳帧
            {
                _frameCount = frameCounter + 1;
                timer = (float) _frameCount / _fps;
            }

            frameCounter = _frameCount;


            //帧前行
            int frame = lastframe + 1;
            if (frame > endframe)
            {
                if (bLooped)
                {
                    frame = startframe;
                }
                else
                {
                    frame = endframe;
                }
            }
            
            //设置动作或者插值
            if (crossend)
            {
                crossFrame = null;
                SetPose(lastClip, frame, true);
                return;
            }

            if (_crossTimer >= 0)
            {
                //_crossTimer -= delta;
                //if (_crossTimer < 0)
                //{
                //    crossFrame = null;
                //    SetPose(lastClip, frame, true);
                //    return;
                //}
                if (crossFrame != null)
                {
                    float l = 1.0f - _crossTimer / _crossTimerTotal;
                    SetPoseLerp(crossFrame, lastClip.frames[frame], l);

                    lastframe = frame;
                    frameNow = lastClip.frames[frame];
                }
            }
            else
            {
                if (frame != lastframe)
                {
                    SetPose(lastClip, frame);
                    frameNow = lastClip.frames[frame];
                }
            }

        }

        int transcode = -1;
        Transform[] trans = null;

        public void SetPose(AniClip clip, int frame, bool reset = false, Transform parent = null)
        {
            if (clip.bonehash != transcode)
            {
                trans = new Transform[clip.boneinfo.Count];
                for (int i = 0; i < clip.boneinfo.Count; i++)
                {
                    trans[i] = this.transform.Find(clip.boneinfo[i]);
                }

                transcode = clip.bonehash;
            }

            bool badd = false;

            if (lastClip == clip && !reset)
            {
                if (lastframe + 1 == frame) badd = transform;
                if (clip.loop && lastframe == clip.frames.Count - 1 && frame == 0)
                    badd = true;
            }

            for (int i = 0; i < trans.Length; i++)
            {
                if (trans[i] == null) continue;
                if (parent != null && parent != trans[i])
                {
                    if (trans[i].IsChildOf(parent) == false) continue;
                }

                clip.frames[frame].bonesinfo[i].UpdateTran(trans[i], badd);
            }

            if (clip.frames.Count > 0 && frame >= 0)
            {
                SetBoxColiderAttribute(clip.frames[frame]); //设置碰撞盒
                if (IsShowBoxLine)
                {
                    SetDebugDot(clip.frames[frame]); //设置触发点
                }

                if (bPlayRunTime)
                {
                    SetEffect(clip.frames[frame]); //设置/检测特效
                    SetAudio(clip.frames[frame]);
                }
            }

            lastClip = clip;
            lastframe = frame;
        }

        class die
        {
            public int effid;
            public int lifetime; //per pose -1;
        }

        List<die> livetimelist = new List<die>();

        public void SetPoseLerp(Frame src, Frame dest, float lerp)
        {
            for (int i = 0; i < trans.Length; i++)
            {
                src.bonesinfo[i].UpdateTranLerp(trans[i], dest.bonesinfo[i], lerp);
            }

            SetBoxColiderAttribute(dest); //设置碰撞盒
            if (IsShowBoxLine)
            {
                SetDebugDot(dest); //设置触发点
            }

            if (bPlayRunTime)
            {
                SetEffect(dest); //设置/检测特效
                SetAudio(dest);
            }
        }



        //EffectMng effectmng = new EffectMng();
        bool bAutoUpdate = true;

        public void SkipAutoUpdate()
        {
            bAutoUpdate = false;
        }

        public void UnSkipAutoUpdate()
        {
            bAutoUpdate = true;
        }

        void Update()
        {
            if (bPlayRunTime && bAutoUpdate)
            {
                _OnUpdate(Time.deltaTime);
            }

            if (ischange != IsShowBoxLine)
            {
                CheckShowBox();
            }
        }

        #region  Box模块

        bool ischange = false;

        void CheckShowBox()
        {
            ischange = IsShowBoxLine;
            if (IsShowBoxLine)
            {
                foreach (var o in mBoxList)
                {
                    if (!o.GetComponent<Collider_Vis>())
                        o.AddComponent<Collider_Vis>();
                    if (!o.GetComponent<LineRenderer>())
                        o.AddComponent<LineRenderer>();
                    if (!o.GetComponent<MeshRenderer>())
                        o.AddComponent<MeshRenderer>();
                    SetBoxColor(o);
                }

                foreach (var o in mDotList)
                {
                    if (!o.GetComponent<Point_Vis>())
                        o.AddComponent<Point_Vis>();
                    if (!o.GetComponent<LineRenderer>())
                        o.AddComponent<LineRenderer>();
                }

                //o.GetComponent<Collider_Vis>().updateColl();
            }
            else
            {
                foreach (var o in mBoxList)
                {
                    if (o.GetComponent<Collider_Vis>())
                        DestroyImmediate(o.GetComponent<Collider_Vis>());
                    if (o.GetComponent<LineRenderer>())
                        DestroyImmediate(o.GetComponent<LineRenderer>());
                    if (o.GetComponent<MeshRenderer>())
                        DestroyImmediate(o.GetComponent<MeshRenderer>());
                }

                foreach (var o in mDotList)
                {
                    if (o.GetComponent<Point_Vis>())
                        DestroyImmediate(o.GetComponent<Point_Vis>());
                    if (o.GetComponent<LineRenderer>())
                        DestroyImmediate(o.GetComponent<LineRenderer>());
                }
            }
        }

        void SetBoxColiderAttribute(Frame src)
        {
            if (_boxes != null)
            {
                _boxes.transform.localPosition = Vector3.zero;
                _boxes.transform.localRotation = new Quaternion(0, 0, 0, 0);
            }

            if (src.boxesinfo != null)
            {
                //剔除null
                for (int i = mBoxList.Count - 1; i >= 0; i--)
                {
                    if (mBoxList[i] == null)
                    {
                        mBoxList.RemoveAt(i);
                    }
                }

                for (int i = 0; i < src.boxesinfo.Count; i++)
                {
                    if (mBoxList.Count - 1 < i)
                    {
                        CreateBox(1);
                    }

                    SetBoxAttribute(src.boxesinfo[i], mBoxList[i]);
                }

                if (mBoxList.Count > src.boxesinfo.Count)
                {
                    for (int i = src.boxesinfo.Count; i < mBoxList.Count; i++)
                    {
                        if (mBoxList[i].activeSelf)
                        {
                            mBoxList[i].SetActive(false);
                        }
                    }
                }
            }
        }

        //对象池
        List<GameObject> mBoxList = new List<GameObject>();
        List<List<GameObject>> mBoxArray = new List<List<GameObject>>();
        GameObject _boxes = null;
        public bool IsShowBoxLine = true;

        void CreateBox(int count)
        {
            if (!transform.Find("_boxes"))
            {
                _boxes = new GameObject("_boxes");
                _boxes.transform.parent = transform;
            }
            else
            {
                _boxes = transform.Find("_boxes").gameObject;
                if (mBoxList.Count == 0)
                {
                    foreach (Transform t in _boxes.transform)
                    {
                        if (t != null)
                        {
                            t.gameObject.SetActive(false);
                            mBoxList.Add(t.gameObject);
                        }
                    }

                    if (mBoxList.Count > 0)
                        return;
                }
            }

            //加载AttackBox
            for (int i = 0; i != count; i++)
            {
                AddBoxTo(_boxes);
            }
        }

        //添加box 
        void AddBoxTo(GameObject father)
        {
            GameObject o = null;
            o = GameObject.CreatePrimitive(PrimitiveType.Cube);
            o.gameObject.name = "BoxColider";
            o.AddComponent<Collider_Vis>();
            var material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
            material.color = new Color(1f, 1f, 1f, 0.2f);
            o.GetComponent<MeshRenderer>().material = material;

            o.transform.parent = father.transform;
            o.SetActive(false);
            o.hideFlags = HideFlags.DontSave;
            if (o != null)
            {
                mBoxList.Add(o);
            }
        }

        void SetBoxAttribute(AniBoxCollider _box, GameObject _curBox = null)
        {
            if (_curBox != null)
                _curBox.transform.localRotation = new Quaternion(0, 0, 0, 0);
            _curBox.SetActive(true);
            //重新设置box的属性
            _curBox.gameObject.name = _box.mName;
            _curBox.layer = LayerMask.NameToLayer(_box.mBoxType);
            //if (_curBox.transform.localPosition != _box.mPosition) 
            //{
            _curBox.transform.localPosition = _box.mPosition;
            //} 
            //计算scale
            var _colider = _curBox.GetComponent<BoxCollider>();
            _curBox.transform.localScale = new Vector3(_box.mSize.x / _colider.size.x, _box.mSize.y / _colider.size.y,
                _box.mSize.z / _colider.size.z);


            if (IsShowBoxLine)
            {
                if (!_curBox.GetComponent<Collider_Vis>())
                    _curBox.AddComponent<Collider_Vis>();
                if (!_curBox.GetComponent<LineRenderer>())
                    _curBox.AddComponent<LineRenderer>();
                if (!_curBox.GetComponent<MeshRenderer>())
                    _curBox.AddComponent<MeshRenderer>();
                SetBoxColor(_curBox);

                //o.GetComponent<Collider_Vis>().updateColl();
            }
            else
            {
                if (_curBox.GetComponent<Collider_Vis>())
                    DestroyImmediate(_curBox.GetComponent<Collider_Vis>());
                if (_curBox.GetComponent<LineRenderer>())
                    DestroyImmediate(_curBox.GetComponent<LineRenderer>());
                if (_curBox.GetComponent<MeshRenderer>())
                    DestroyImmediate(_curBox.GetComponent<MeshRenderer>());
            }
        }

        public class BoxColor
        {
            public BoxColor(Color line, Color box)
            {
                linecolor = line;
                boxcolor = box;
            }

            public Color linecolor;
            public Color boxcolor;
        }

        public Dictionary<string, BoxColor> boxcolor = new Dictionary<string, BoxColor>()
        {
            {"box_attack", new BoxColor(Color.black, new Color(0f, 0f, 0f, 0.3f))},
            {"box_area", new BoxColor(Color.green, new Color(0f, 1f, 0f, 0.3f))},
            {"box_behurt", new BoxColor(Color.red, new Color(1f, 0f, 0f, 0.3f))}
        };

        public void SetBoxColor(GameObject _curBox)
        {
            if (IsShowBoxLine)
            {
                //颜色
                Collider_Vis collider_Vis = null;
                LineRenderer lineRenderer = null;
                MeshRenderer meshRenderer = null;

                if (_curBox.GetComponent<MeshRenderer>() && _curBox.GetComponent<Collider_Vis>())
                {
                    collider_Vis = _curBox.GetComponent<Collider_Vis>();
                    meshRenderer = _curBox.GetComponent<MeshRenderer>();
                    collider_Vis.linewidth = 0.2f;
                    collider_Vis.updateColl();
                }

                lineRenderer = _curBox.GetComponent<LineRenderer>();

                var material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));

                if (boxcolor.ContainsKey(LayerMask.LayerToName(_curBox.layer))) //Attck
                {
                    if (collider_Vis != null)
                    {
                        collider_Vis.lineColor = boxcolor[LayerMask.LayerToName(_curBox.layer)].linecolor;
                    }

                    material.color = boxcolor[LayerMask.LayerToName(_curBox.layer)].boxcolor;
                }

                meshRenderer.enabled = true;
                lineRenderer.enabled = true;
                meshRenderer.material = material;
                collider_Vis.updateColl();
            }
        }

        #endregion

        #region 触发点

        List<GameObject> mDotList = new List<GameObject>();
        GameObject dot;

        public GameObject CreateDot()
        {
            if (transform.Find("_dotes"))
            {
                dot = transform.Find("_dotes").gameObject;
            }
            else
            {
                dot = new GameObject("_dotes");
            }

            dot.transform.parent = transform;
            dot.transform.localPosition = Vector3.zero;
            dot.transform.localRotation = new Quaternion(0, 0, 0, 0);
            dot.transform.localScale = Vector3.one;
            GameObject o = new GameObject();
            o.transform.localScale = new Vector3(3, 3, 3);
            o.AddComponent<Point_Vis>();
            o.GetComponent<Point_Vis>().UpdatePoint();
            o.transform.parent = dot.transform;
            mDotList.Add(o);
            return o;
        }

        public void SetDotAttribute(Dot d)
        {
            for (int i = mDotList.Count - 1; i >= 0; i--)
            {
                if (mDotList[i] == null)
                {
                    mDotList.RemoveAt(i);
                }
            }

            if (dot != null)
                dot.transform.localRotation = new Quaternion(0, 0, 0, 0);
            GameObject _curdot = mDotList.Find(o => !o.activeSelf);
            if (_curdot == null)
            {
                _curdot = CreateDot();
            }

            _curdot.SetActive(true);
            _curdot.transform.localPosition = d.position;
            _curdot.name = d.name;
            switch (d.name)
            {
                case "hold":
                    _curdot.GetComponent<Point_Vis>().lineColor = Color.black;
                    break;
                case "behold":
                    _curdot.GetComponent<Point_Vis>().lineColor = Color.red;
                    break;
                case "create":
                    _curdot.GetComponent<Point_Vis>().lineColor = Color.green;
                    break;
            }

            _curdot.GetComponent<Point_Vis>().UpdatePoint();
        }

        void ReturnDot()
        {
            for (int i = mDotList.Count - 1; i >= 0; i--)
            {
                if (mDotList[i] == null)
                {
                    mDotList.RemoveAt(i);
                }
            }

            foreach (var o in mDotList)
            {
                if (o != null)
                {
                    o.SetActive(false);
                }
            }
        }

        void SetDebugDot(Frame f)
        {
            ReturnDot(); //每一帧调用，先重置box
            if (f.boxesinfo != null)
            {
                foreach (var b in f.dotesinfo)
                {
                    SetDotAttribute(b);
                }
            }
        }

        #endregion

        #region 特效

        //每帧检测
        void SetEffect(Frame f)
        {
            CheckEffect();
            foreach (var e in f.effectList)
            {
                Transform o = this.transform.Find(e.follow);
                if (e.lifeframe > 0)
                {
                    if (o != null)
                    {
                        die d = new die();
                        d.lifetime = e.lifeframe;
                        d.effid = AniResource.PlayEffectLooped(e.name, e.position, dir, o);
                        livetimelist.Add(d);
                    }
                }
                else
                {
                    AniResource.PlayEffect(e.name, o, e.position, e.isFollow, dir);
                }
            }
        }

        //管理生命周期
        void CheckEffect()
        {
            //编辑器中误操作，移除所有为null的引用
            for (int i = livetimelist.Count - 1; i >= 0; i--)
            {
                livetimelist[i].lifetime--; //生命周期每帧 -1

                if (livetimelist[i].lifetime <= 0) //生命周期结束 删除特效
                {
                    AniResource.CloseEffectLooped(livetimelist[i].effid);
                    livetimelist.RemoveAt(i);
                }
            }

            //Resources.UnloadUnusedAssets();
            //GC.Collect();
        }

        #endregion

        #region  音效

        void SetAudio(Frame f)
        {
            foreach (var audio in f.aduioList)
            {
                AniResource.PlaySoundOnce(audio);
            }         
        }

        #endregion

        public int dir = 1;
        int dircur;

        public int chardir
        {
            get { return dir; }
            set { dir = chardir; }
        }

        public void SetDir(int dir)
        {
            this.dir = dircur = dir;
            this.transform.LookAt(this.transform.position + new Vector3(dir == 1 ? 1 : -1, 0, 0), Vector3.up);
        }

        string curClipName;

        public void Play(string clip, string subclip = null, float cross = 0.2f)
        {
            if (string.IsNullOrEmpty(clip) == false)
            {
                var _clip = GetClip(clip);
                if (_clip == null)
                {
                    Debug.LogWarning("No clip:" + clip);
                    return;
                }

                SubClip _subclip = null;
                if (string.IsNullOrEmpty(subclip) == false)
                {
                    _subclip = _clip.GetSubClip(subclip);
                }

                //清除特效
                if (!string.IsNullOrEmpty(curClipName))
                    if (curClipName.Equals(clip))
                    {
                        foreach (var i in livetimelist)
                        {
                            i.lifetime = 1000;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < livetimelist.Count - 1; i++)
                        {
                            AniResource.CloseEffectLooped(livetimelist[i].effid);
                            livetimelist.RemoveAt(i);
                        }

                        AniResource.CleanAllEffect();

                        curClipName = clip;
                    }

                //Debug.LogError("_clip = " + _clip);
                Play( clip:_clip, clipsub: _subclip, crosstimer: cross);
            }
        }


        //临时放在这里的公用变量
        public Vector3 wantpos;

        //bool ispause;
        int pauseframe = 0;

        //int pausecount;
        public bool IsPause
        {
            get { return pauseframe > 0; }
        }

        public void PlayPause(int frame)
        {
            pauseframe = frame;
            //ispause = true;
            //pausecount = 0;
        }




      
    }
}