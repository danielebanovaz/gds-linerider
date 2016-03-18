/*                                                                                     *\
           _____ _____   _____   _      _              _____  _     _           
          / ____|  __ \ / ____| | |    (_)            |  __ \(_)   | |          
         | |  __| |  | | (___   | |     _ _ __   ___  | |__) |_  __| | ___ _ __ 
         | | |_ | |  | |\___ \  | |    | | '_ \ / _ \ |  _  /| |/ _` |/ _ \ '__|
         | |__| | |__| |____) | | |____| | | | |  __/ | | \ \| | (_| |  __/ |   
          \_____|_____/|_____/  |______|_|_| |_|\___| |_|  \_\_|\__,_|\___|_|   
      ˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽
        ©2016 Sionera Entertainment - Daniele Banovaz (daniele.banovaz@gmail.com)

        Developed as a tutorial for Game Developement Saturday #1, 2016-03-19, PN

\*                                                                                     */


using System;
using UnityEngine;

/// <summary>
/// A Coin that can be collected by running Car
/// </summary>
public class Coin : MonoBehaviour
{
    /// <summary>
    /// Color of the rare Mega Coin
    /// </summary>
    public static readonly Color MegaCoinColor = new Color(1, 0.3f, 0f);

    /// <summary>
    /// If true, this is considered a "Mega Coin": bigger, rarer and more valuable
    /// </summary>
    public bool MegaCoin
    {
        get { return _megaCoin; }
        set
        {
            _megaCoin = value;
            _spriteRenderer.color = _megaCoin ? MegaCoinColor : Color.white;
            transform.localScale = _megaCoin ? Vector3.one * 1.5f : Vector3.one;
        }
    }

    /// <summary>
    /// Backing field for MegaCoin
    /// </summary>
    private bool _megaCoin;

    /// <summary>
    /// Triggered when this Coin is being collected by the Player
    /// </summary>
    public event Action<Coin> Collected;

    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Called by Unity when this Component is created
    /// </summary>
    private void Awake()
    {
        // It's a good practice to link components during initialization,
        // to avoid performance drawbacks in repeated searches
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Called by Unity when a physical 2D collider impacts with this Coin
    /// </summary>
    /// <param name="collidingEntity"></param>
    private void OnTriggerEnter2D(Collider2D collidingEntity)
    {
        // Notify event subscribers
        if (Collected != null)
            Collected.Invoke(this);

        // Make this Coin transparent, to let the Player know that
        // it has been successfully collected during previous attempt
        Color fadedColor = _spriteRenderer.color;
        fadedColor.a = 0.25f;
        _spriteRenderer.color = fadedColor;

        // Disable this Coin
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Remove faded out appearance from collected Coins
    /// </summary>
    public void ResetColor()
    {
        _spriteRenderer.color = _megaCoin ? MegaCoinColor : Color.white;
    }
}
