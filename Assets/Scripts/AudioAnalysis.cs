using UnityEngine;
using System.Collections;

public class AudioAnalysis : MonoBehaviour {
	private const float CAP_HEIGHT = 0.03f;
	private const float DECAY_RATE = 0.001f;
	private const float BAR_SCALE = 50;
	private const float DIR_DECAY = 0.001f;

	private AudioSource source;
	private float[] freqData;
	private float[] band;
	private float[] buffer;
	private float wubPower;
	private float wubDir;
	private float bigWub;

	private GameObject[] bars;
	private GameObject[] caps;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		
		wubPower = 0;
		bigWub = 0;
		wubDir = 1;

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
		buffer = new float[barCount];
		bars = new GameObject[barCount];
		caps = new GameObject[barCount];
		float halfBar = (float)(barCount-1) / 2f;
		for(int i = 0 ; i < barCount ; i++)
		{
			band[i] = 0;
			buffer[i] = 0;

			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			obj.transform.position = new Vector3(((float)i)-halfBar,0,5);
			bars[i] = obj;
			
			GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			cap.transform.position = new Vector3(((float)i)-halfBar,0,5);
			cap.transform.localScale = new Vector3(1,CAP_HEIGHT,1);
			cap.renderer.material.color = Color.red;
			caps[i] = cap;
		}

	}
	
	// Update is called once per frame
	void Update () {
		UpdateSpectrum();
		string dubString = "";
		for(int i = 0 ; i < bars.Length ; i++)
		{
			float newY = band[i] * BAR_SCALE;
			Vector3 pos = bars[i].transform.position;
			bars[i].transform.position = new Vector3(pos.x,newY,pos.z);
			bars[i].transform.localScale = new Vector3(1,newY,1);

			newY = buffer[i] * BAR_SCALE;
			pos = caps[i].transform.position;
			caps[i].transform.position = new Vector3(pos.x, (newY *2) + CAP_HEIGHT, pos.z);

			wubPower += (band[i]) * (Mathf.Pow (i,2));

			dubString += i + " " + band[i] + " | ";

			band[i] = 0;
			buffer[i] = Mathf.Max (buffer[i]-DECAY_RATE,0);
		}
		if(wubPower > bigWub)
		{
			bigWub = wubPower;
			wubDir *= -1;
		}else{
			bigWub -= DECAY_RATE;
		}
		wubPower *= wubDir;
		camera.transform.Rotate(0,0,wubPower);

		Debug.Log (dubString + "WubPower: " + wubPower + " | BigWub: " + bigWub);

		wubPower = 0;
	}

	void UpdateSpectrum(){
		source.GetSpectrumData(freqData,0,FFTWindow.Rectangular);
		int k = 0;
		int cross = 2;
		for(int i = 0 ; i < freqData.Length ; i++)
		{
			band[k] = Mathf.Max (freqData[i],band[k]);
			if(band[k] > buffer[k])
			{
				FireBeat(k,buffer[k],band[k]);
				buffer[k] = band[k];
			}
			if(i > cross-3)
			{
				k++;
				cross *= 2;
			}
		}
	}

	private void FireBeat(int column, float oldMax, float newMax)
	{

	}
}
