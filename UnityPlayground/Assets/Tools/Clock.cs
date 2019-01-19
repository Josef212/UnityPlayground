using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GG
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private GameObject m_visual = null;
        [SerializeField] private Text m_clockText = null;

        private float m_timer;
        private Action m_callback = null;
        private bool m_running;

        public void StartClock(float time, Action callback)
        {
            m_timer = time;
            m_callback = callback;
            m_running = true;
            m_visual.SetActive(true);

            UpdateText();
        }

        public void StopClock()
        {
            m_running = false;
            m_visual.SetActive(false);
            m_callback = null;
        }

        private void Awake()
        {
            StopClock();
        }

        private void Update()
        {
            if(m_running)
            {
                m_timer -= Time.deltaTime;

                if(m_timer <= 0)
                {
                    m_timer = 0;
                    m_running = false;
                    if (m_callback != null) m_callback();
                    m_callback = null;
                }

                UpdateText();
            }
        }

        private void UpdateText()
        {
            int minutes = (int)(m_timer / 60.0f);
            int seconds = (int)(m_timer % 60.0f);
            string zero = (seconds < 10) ? "0" : "";

            m_clockText.text = minutes + ":" + zero + seconds;
        }
    }
}