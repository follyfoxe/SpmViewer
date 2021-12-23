using System.Collections;
using UnityEngine;

namespace PM.Models.Animations
{
    public class PMAnimator : MonoBehaviour
    {
        public PMAnimation Animation { get; private set; }
        public ModelAnimCurves Curves;

        public bool IsPlaying { get; private set; }
        float StartTime;
        float Duration;

        public void SetAnimation(PMAnimation anim)
        {
            Animation = anim;

            DeltaCurve c = new(anim);
            c.Bake();
            Curves = new ModelAnimCurves(Animation.Data.Keyframes, c.Frames);
        }

        public void Play()
        {
            if (IsPlaying)
                Stop();

            Animation.Model.ResetVirtualModel();
            IsPlaying = true;
            StartTime = Time.time;
            Duration = Animation.GetDuration();
        }

        public void Stop()
        {
            if (IsPlaying)
                IsPlaying = false;
        }

        void Update()
        {
            if (IsPlaying)
            {
                float time = (Time.time - StartTime) * 40f;
                if (Animation.Loop)
                    time %= Duration;
                else
                {
                    if (time > Duration)
                    {
                        Stop();
                        return;
                    }
                }
                Curves.Evaluate(Animation.Model, time);
            }
        }
    }
}