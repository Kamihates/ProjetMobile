using GooglePlayGames;
using UnityEngine;

public class GooglePlayManager : MonoBehaviour
{
    public void Start()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.localUser.Authenticate(ProcessAuthentication);
    }

    void ProcessAuthentication(bool success)
    {
        Debug.Log("identification réussie = " + success);
    }

}
