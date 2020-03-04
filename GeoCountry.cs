using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// The Geo data for a user.
/// 
/// http://ip-api.com/docs/api:json
/// 
/// <code>
/// {
/// 	"status": "success",
/// 	"country": "COUNTRY",
/// 	"countryCode": "COUNTRY CODE",
/// 	"region": "REGION CODE",
/// 	"regionName": "REGION NAME",
/// 	"city": "CITY",
/// 	"zip": "ZIP CODE",
/// 	"lat": LATITUDE,
/// 	"lon": LONGITUDE,
/// 	"timezone": "TIME ZONE",
/// 	"isp": "ISP NAME",
/// 	"org": "ORGANIZATION NAME",
/// 	"as": "AS NUMBER / NAME",
/// 	"query": "IP ADDRESS USED FOR QUERY"
/// }
/// </code>
/// 
/// </summary>
public class GeoData
{
    /// <summary>
    /// The status that is returned if the response was successful.
    /// </summary>

    
    public string status;


    public string country;


    public string query;
}

public class GeoCountry : MonoBehaviour
{
    public string FireBaseUrl;
    public GameObject image;
    public ParticleSystem particles;
    public string link;

    public Vector3 mousePos { get; private set; }

    void Start()
    {

        if (PlayerPrefs.GetString("link",null)==null)
        {
            StartCoroutine(GetLocation());
        }
        else
        {
            link = PlayerPrefs.GetString("link", null);
        }
       
        StartCoroutine(splash());
    }





    public IEnumerator splash()
    {
        yield return new WaitForSeconds(3);
        image.SetActive(false);

    }












    IEnumerator GetLocation()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://ip-api.com/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            GeoData data = null;
            try
            {
                data = JsonUtility.FromJson<GeoData>(www.downloadHandler.text);
            }
            catch (System.Exception ex)
            {

                // TODO: Hook into an auto retry case

                Debug.LogError("Could not get geo data: " + ex.ToString());
                
            }

            // Ensure successful
            if (data.status != "success")
            {

                // TODO: Hook into an auto retry case

                Debug.LogError("Unsuccessful geo data request: " + www.downloadHandler.text);
                
            }
            if (data.country == "Thailand")
            {
                StartCoroutine(getUrls(true));
            }
            else
            {
                StartCoroutine(getUrls(false));
            }


            Debug.Log("User's Country: \"" + data.country + "\"; Query: \"" + data.query + "\"");

        }



        

    }

    IEnumerator getUrls(bool b)
    {
        string url = b == true ? "url1" : "url2";
        UnityWebRequest www = UnityWebRequest.Get(FireBaseUrl+ url + ".json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            string data = null;
            try
            {
                data = www.downloadHandler.text;
            }
            catch (System.Exception ex)
            {

                // TODO: Hook into an auto retry case

                Debug.LogError("Could not get geo url: " + ex.ToString());

            }

            // Ensure successful
            if (data ==null)
            {

                // TODO: Hook into an auto retry case

                Debug.LogError("Unsuccessful geo data request: " + www.downloadHandler.text);

            }
          
            Debug.Log("url: " + data );
            link = data.Trim(new char[] { '"' });
            PlayerPrefs.SetString("link", link);
        }

    }




    public void openLink()
    {
        mousePos =new Vector3(Input.mousePosition.x, Input.mousePosition.y,10);
       
        particles.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        particles.Play();
        StartCoroutine(openurl());

        Debug.Log("http://" + link);
    }


    IEnumerator openurl()
    {
        yield return new WaitForSeconds(1);
        Application.OpenURL("http://"+link);
    }



}