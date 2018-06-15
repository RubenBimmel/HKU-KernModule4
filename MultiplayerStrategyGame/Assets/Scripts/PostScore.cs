using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PostScore : MonoBehaviour {

    private static PostScore instance;
    private static string session;

    public static PostScore GetInstance () {
        if (!instance) {
            instance = new GameObject().AddComponent<PostScore>();
        }
        return instance;
    }

    private void Start() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Post() {
        if (PlayerPrefs.HasKey("score")) {
            StartCoroutine(PostPlayerScore(PlayerPrefs.GetInt("score")));
        }
    }

    IEnumerator PostPlayerScore(int score) {
        //First we create a form to add information to
        WWWForm form = new WWWForm();
        form.AddField("gameid", 6);
        form.AddField("score", score);

        //We're using Post by default, because we're always sending sessionID
        string url = "http://studenthome.hku.nl/~ruben.bimmel/HKU/jaar2dev/Kernmodule4/database/posttosession.php";

        //Pass the current session id if there is one active
        if (session != null) {
            url += "?sessionid=" + session;
        }

        //Post game data
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        //This blocks
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            //Save session id
            session = www.downloadHandler.text;

            //Go to confirmation page
            Application.OpenURL("http://studenthome.hku.nl/~ruben.bimmel/HKU/jaar2dev/Kernmodule4/database/confirmfromgame.php?sessionid=" + session);
            PlayerPrefs.DeleteKey("score");
        }

        yield return null;
    }
}
