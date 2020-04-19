using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI: MonoBehaviour
{
    public Text objectiveDescription;
    public Text progressText;
    public Slider progressSlider;
    public Objective currentObjective;

    public void SetObjective(Objective objective)
    {
        currentObjective = objective;
        gameObject.SetActive(currentObjective);

        if (currentObjective)
        {
            objectiveDescription.text = currentObjective.description;
            Update();
        }
    }

    private void Update()
    {
        var now = Time.time;
        progressSlider.gameObject.SetActive(currentObjective.IsBusy);
        progressSlider.value = currentObjective.GetProgressFractionAt(now);
        progressText.text = $"{Mathf.CeilToInt(currentObjective.GetTimeRemainingAt(now))}s";
    }
}
