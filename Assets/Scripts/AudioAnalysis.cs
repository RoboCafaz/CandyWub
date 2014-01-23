using UnityEngine;
using System.Collections;

public class AudioAnalysis : MonoBehaviour {
	private AudioSource source;
	private float[] freqData;
	private float[] band;

	private GameObject[] bars;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		freqData = new float[8192];
		int n = freqData.Length;
		int barCount = 1;
		for(int i = 0 ; i < freqData.Length ; i++)
		{
			n /= 2;
			if(n == 0)
			{
				break;
			}
			barCount++;
		}
		band = new float[barCount];
		bars = new GameObject[barCount];
		float halfBar = (float)(barCount-1) / 2f;
		for(int i = 0 ; i < barCount ; i++)
		{
			band[i] = 0;
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			obj.transform.position = new Vector3(((float)i)-halfBar,0,5);
			bars[i] = obj;
		}

	}
	
	// Update is called once per frame
	void Update () {
		UpdateSpectrum();
		for(int i = 0 ; i < bars.Length ; i++)
		{
			float newY = band[i] * 50;
			Vector3 pos = bars[i].transform.position;
			bars[i].transform.position = new Vector3(pos.x,newY,pos.z);
			bars[i].transform.localScale = new Vector3(1,newY,1);
			band[i] = 0;
		}
	}

	void UpdateSpectrum(){
		source.GetSpectrumData(freqData,0,FFTWindow.Rectangular);
		int k = 0;
		int cross = 2;
		for(int i = 0 ; i < freqData.Length ; i++)
		{
			float d = freqData[i];
			float b = band[k];
			band[k] = Mathf.Max (d,b);
			if(i > cross-3)
			{
				k++;
				cross *= 2;
			}
		}
	}
}
