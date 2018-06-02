using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GhitarControl : MonoBehaviour
{
    [SerializeField] private float delayCheck;
    [SerializeField] private Visualizer visualizer;
    [SerializeField] private KeyCode[] keycodes;

    [SerializeField] private Image[] keyImages;
    [SerializeField] private Text txtScore;

    private KeyCode currentCorrectKey;
    private int barCount;
    private int score;
    private float totalDeltaTime;
    bool skipFrame = false;

    // Use this for initialization
    void Start()
    {
        barCount = keycodes.Length;

        Debug.Assert(barCount == visualizer.spectrums.Length, "This should be equal!!!");
    }
    

    // Update is called once per frame
    void Update()
    {
        totalDeltaTime += Time.deltaTime;

        if (totalDeltaTime > delayCheck)
        {
            totalDeltaTime = 0f;

            if (visualizer.vector3.magnitude < .05f)
            {
                skipFrame = true;
            }
            else
            {
                skipFrame = false;
                var spectrums = visualizer.spectrums;
                var spectrum = visualizer.vector4;
                var correctIndex = spectrums.ToList().IndexOf(spectrums.Max());
                currentCorrectKey = keycodes[correctIndex];

                print("Correct key: " + currentCorrectKey);
            
                //reset image colors
                keyImages.ToList().ForEach(img => img.color = new Color(1, 1, 1, .5f));

                //highlight the key for users
                keyImages[correctIndex].color = new Color(spectrum.x, spectrum.y, spectrum.z, 1f);
            }
           
        }

        if (skipFrame)
        {
            keyImages.ToList().ForEach(img => img.color = new Color(1, 1, 1, 0f));
            return;
        }
        
        foreach (var keycode in keycodes)
        {
            if (Input.GetKeyDown(keycode))
            {
                if (currentCorrectKey == keycode)
                {
                    skipFrame = true;
                    score++;
                    txtScore.color = Color.green;
                    txtScore.text = score.ToString();
                    Debug.Log("Correct key");
                    break;
                }
                else
                {
                    score--;
                    txtScore.color = Color.red;
                    txtScore.text = score.ToString();
                    Debug.Log("Wrong key");
                }
                

            }
        }
    }
}