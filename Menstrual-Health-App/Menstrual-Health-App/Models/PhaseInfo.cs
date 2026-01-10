namespace Menstrual_Health_App.Models;

public class PhaseInfo
{
    public CyclePhase Phase { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public Color PhaseColor { get; set; } = Colors.Gray;
    public List<string> Symptoms { get; set; } = new();
    public List<string> HealthTips { get; set; } = new();
    public List<string> NutritionTips { get; set; } = new();
    public List<string> ExerciseTips { get; set; } = new();
    public List<string> SelfCareTips { get; set; } = new();

    public static PhaseInfo GetPhaseInfo(CyclePhase phase)
    {
        return phase switch
        {
            CyclePhase.Menstrual => GetMenstrualPhaseInfo(),
            CyclePhase.Follicular => GetFollicularPhaseInfo(),
            CyclePhase.Ovulation => GetOvulationPhaseInfo(),
            CyclePhase.Luteal => GetLutealPhaseInfo(),
            _ => GetMenstrualPhaseInfo()
        };
    }

    private static PhaseInfo GetMenstrualPhaseInfo()
    {
        return new PhaseInfo
        {
            Phase = CyclePhase.Menstrual,
            Title = "Menstrual Phase",
            Description = "Your period has started. This is the beginning of your cycle when the uterine lining sheds. It's normal to experience some discomfort, but this is also a time for rest and renewal.",
            Icon = "P",
            PhaseColor = Color.FromArgb("#E74C3C"),
            Symptoms = new List<string>
            {
                "Menstrual bleeding",
                "Cramping or abdominal pain",
                "Lower back pain",
                "Fatigue and low energy",
                "Bloating",
                "Headaches",
                "Mood changes"
            },
            HealthTips = new List<string>
            {
                "Stay hydrated - drink at least 8 glasses of water daily",
                "Apply heat to your lower abdomen for cramp relief",
                "Get adequate sleep (7-9 hours)",
                "Take iron supplements if recommended by your doctor",
                "Track your symptoms for future reference"
            },
            NutritionTips = new List<string>
            {
                "Eat iron-rich foods: leafy greens, lean red meat, beans",
                "Include vitamin C to enhance iron absorption",
                "Avoid excessive caffeine and salt",
                "Eat anti-inflammatory foods: fatty fish, berries, turmeric",
                "Dark chocolate (in moderation) can help with cravings and provide magnesium"
            },
            ExerciseTips = new List<string>
            {
                "Light walking or stretching",
                "Gentle yoga poses (child's pose, cat-cow)",
                "Swimming can help relieve cramps",
                "Listen to your body and rest when needed",
                "Avoid intense workouts if experiencing heavy bleeding"
            },
            SelfCareTips = new List<string>
            {
                "Take warm baths to ease discomfort",
                "Practice deep breathing or meditation",
                "Journal your thoughts and feelings",
                "Use comfortable, loose-fitting clothing",
                "Be patient with yourself during this time"
            }
        };
    }

    private static PhaseInfo GetFollicularPhaseInfo()
    {
        return new PhaseInfo
        {
            Phase = CyclePhase.Follicular,
            Title = "Follicular Phase",
            Description = "Your body is preparing for ovulation. Estrogen levels are rising, and you may notice increased energy and improved mood. This is often considered the 'spring' of your cycle.",
            Icon = "F",
            PhaseColor = Color.FromArgb("#E91E63"),
            Symptoms = new List<string>
            {
                "Increased energy levels",
                "Improved mood and optimism",
                "Higher concentration and focus",
                "Increased libido",
                "Clearer skin",
                "Better sleep quality"
            },
            HealthTips = new List<string>
            {
                "Take advantage of your increased energy",
                "Schedule important tasks and meetings",
                "Start new projects or goals",
                "Focus on building healthy habits",
                "Stay active and maintain a regular exercise routine"
            },
            NutritionTips = new List<string>
            {
                "Eat fresh, light foods: salads, fermented foods",
                "Include lean proteins for energy",
                "Eat plenty of fruits and vegetables",
                "Include healthy fats: avocados, nuts, olive oil",
                "Probiotic-rich foods support gut health"
            },
            ExerciseTips = new List<string>
            {
                "Great time for high-intensity workouts",
                "Try new fitness classes or activities",
                "Strength training is highly effective now",
                "Cardio exercises: running, cycling, dancing",
                "Your body recovers faster during this phase"
            },
            SelfCareTips = new List<string>
            {
                "Socialize and connect with friends",
                "Try new experiences and activities",
                "Set goals and plan for the month ahead",
                "Focus on creative projects",
                "Take on challenges you've been postponing"
            }
        };
    }

    private static PhaseInfo GetOvulationPhaseInfo()
    {
        return new PhaseInfo
        {
            Phase = CyclePhase.Ovulation,
            Title = "Ovulation Phase",
            Description = "This is your most fertile window. An egg is released from the ovary. You may feel your most confident and social during this time. Energy levels are typically at their peak.",
            Icon = "O",
            PhaseColor = Color.FromArgb("#FF9800"),
            Symptoms = new List<string>
            {
                "Peak energy and vitality",
                "Increased libido",
                "Possible mild pelvic discomfort (mittelschmerz)",
                "Changes in cervical mucus",
                "Slight rise in body temperature",
                "Enhanced mood and confidence",
                "Heightened senses"
            },
            HealthTips = new List<string>
            {
                "Track ovulation signs if trying to conceive",
                "Be aware of your fertile window",
                "Monitor any unusual symptoms",
                "Stay well-hydrated",
                "Maintain regular sleep schedule"
            },
            NutritionTips = new List<string>
            {
                "Eat antioxidant-rich foods: berries, colorful vegetables",
                "Include zinc-rich foods: pumpkin seeds, chickpeas",
                "Light, energizing meals work well",
                "Stay hydrated with water and herbal teas",
                "B-vitamins support energy: whole grains, eggs"
            },
            ExerciseTips = new List<string>
            {
                "Peak performance time for intense workouts",
                "Great for competitions or personal records",
                "High-energy group classes",
                "Take advantage of increased stamina",
                "Mix cardio with strength training"
            },
            SelfCareTips = new List<string>
            {
                "Great time for important conversations",
                "Network and build connections",
                "Express creativity",
                "Go on dates or plan social events",
                "Take photos - you may look your best!"
            }
        };
    }

    private static PhaseInfo GetLutealPhaseInfo()
    {
        return new PhaseInfo
        {
            Phase = CyclePhase.Luteal,
            Title = "Luteal Phase",
            Description = "The post-ovulation phase when progesterone rises. You may start to feel more introspective and notice PMS symptoms as your period approaches. This is a time for winding down and self-care.",
            Icon = "L",
            PhaseColor = Color.FromArgb("#9C27B0"),
            Symptoms = new List<string>
            {
                "PMS symptoms may begin",
                "Mood swings or irritability",
                "Breast tenderness",
                "Bloating and water retention",
                "Food cravings",
                "Fatigue",
                "Difficulty concentrating",
                "Headaches"
            },
            HealthTips = new List<string>
            {
                "Prioritize sleep and rest",
                "Reduce stress where possible",
                "Track PMS symptoms to understand patterns",
                "Consider natural remedies for PMS",
                "Consult a doctor if symptoms are severe"
            },
            NutritionTips = new List<string>
            {
                "Complex carbohydrates help with serotonin",
                "Magnesium-rich foods: dark chocolate, spinach, almonds",
                "Calcium can help reduce PMS symptoms",
                "Reduce salt to minimize bloating",
                "Limit caffeine and alcohol",
                "Eat regular, balanced meals to stabilize blood sugar"
            },
            ExerciseTips = new List<string>
            {
                "Moderate exercise helps with PMS",
                "Yoga and Pilates are great options",
                "Walking in nature can improve mood",
                "Reduce intensity as period approaches",
                "Focus on stretching and flexibility"
            },
            SelfCareTips = new List<string>
            {
                "Practice stress-management techniques",
                "Spend time on comforting activities",
                "Organize and prepare for the week ahead",
                "Finish projects rather than starting new ones",
                "Give yourself grace if you're feeling emotional",
                "Create a cozy, comfortable environment"
            }
        };
    }
}
