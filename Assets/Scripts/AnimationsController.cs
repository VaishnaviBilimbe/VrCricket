using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsController : MonoBehaviour
{
    [SerializeField] private List<Animator> _opponents = new List<Animator>();
    [SerializeField] private GameObject _crowd1GameObject;
    [SerializeField] private GameObject _crowd2GameObject;
    [SerializeField] private List<Animator> _umpires = new List<Animator>();
    
    // Lists to store collected animators from crowd hierarchies
    private List<Animator> _crowd1Animators = new List<Animator>();
    private List<Animator> _crowd2Animators = new List<Animator>();

    [Header("Audio")] 
    [SerializeField] private AudioSource _umpire;
    [SerializeField] private AudioSource _reaction;
    [SerializeField] private AudioClip _fourUmpire;
    [SerializeField] private AudioClip _sixUmpire;
    [SerializeField] private AudioClip _wicketUmpire;
    [SerializeField] private AudioClip _winCrowd;
    [SerializeField] private AudioClip _loseCrowd;
    [SerializeField] private AudioClip _winUmpire;
    [SerializeField] private AudioClip _loseUmpire;

    private void Awake()
    {
        // Collect all animators from crowd hierarchies
        if (_crowd1GameObject != null)
            CollectAnimatorsRecursively(_crowd1GameObject, _crowd1Animators);
        
        if (_crowd2GameObject != null)
            CollectAnimatorsRecursively(_crowd2GameObject, _crowd2Animators);
            
        Debug.Log($"Collected {_crowd1Animators.Count} animators from Crowd1 and {_crowd2Animators.Count} from Crowd2");
    }
    
    private void CollectAnimatorsRecursively(GameObject parent, List<Animator> animatorList)
    {
        // Add animator component if it exists on this object
        Animator animator = parent.GetComponent<Animator>();
        if (animator != null)
            animatorList.Add(animator);
            
        // Process all children recursively
        foreach (Transform child in parent.transform)
        {
            CollectAnimatorsRecursively(child.gameObject, animatorList);
        }
    }

    public void AnimateAllCharacters(string triggerName, List<Animator> characterAnimators)
    {
        foreach (Animator animator in characterAnimators)
        {
            if (animator != null)
            {
                animator.SetTrigger(triggerName);
            }
        }
    }

    public void four()
    {
        AnimateAllCharacters("isAngry", _opponents);
        AnimateAllCharacters("isExcited", _crowd1Animators);
        AnimateAllCharacters("isAngry1", _crowd2Animators);
        AnimateAllCharacters("is4", _umpires);
        _umpire?.PlayOneShot(_fourUmpire);
        _reaction?.PlayOneShot(_winCrowd);
    }

    public void six()
    {
        AnimateAllCharacters("isAngry", _opponents);
        AnimateAllCharacters("isExcited", _crowd1Animators);
        AnimateAllCharacters("isExcited1", _crowd2Animators);
        AnimateAllCharacters("is6", _umpires);
        _umpire?.PlayOneShot(_sixUmpire);
        _reaction?.PlayOneShot(_winCrowd);
    }

    public void wicket()
    {
        AnimateAllCharacters("isExcited", _opponents);
        AnimateAllCharacters("isAngry", _crowd1Animators);
        AnimateAllCharacters("isAngry1", _crowd2Animators);
        AnimateAllCharacters("isOut", _umpires);
        _umpire?.PlayOneShot(_wicketUmpire);
        _reaction?.PlayOneShot(_loseCrowd);
    }
    
    public void gameStart()
    {
        AnimateAllCharacters("isExcited", _crowd1Animators);
        AnimateAllCharacters("isAngry1", _crowd2Animators);
        _reaction?.PlayOneShot(_winCrowd);
    }
    
    public void gameWin()
    {
        AnimateAllCharacters("isAngry", _opponents);
        AnimateAllCharacters("isExcited", _crowd1Animators);
        AnimateAllCharacters("isExcited1", _crowd2Animators);
        AnimateAllCharacters("is6", _umpires);
        _umpire?.PlayOneShot(_winUmpire);
        _reaction?.PlayOneShot(_winCrowd);
    }

    public void gameLose()
    {
        AnimateAllCharacters("isExcited", _opponents);
        AnimateAllCharacters("isAngry", _crowd1Animators);
        AnimateAllCharacters("isAngry1", _crowd2Animators);
        AnimateAllCharacters("isOut", _umpires);
        _umpire?.PlayOneShot(_loseUmpire);
        _reaction?.PlayOneShot(_loseCrowd);
    }
}