using System.Collections.Generic;

namespace jwt_rest_api.Common;

public class IpipQuestion
{
    public int ItemNumber { get; set; }
    public string Text { get; set; } = null!;
    public string Trait { get; set; } = null!; // N, E, O, A, C
    public string Facet { get; set; } = null!; // N1, E2, etc.
    public bool IsReverse { get; set; }
}

public static class IpipNeo60Dictionary
{
    public static readonly List<IpipQuestion> Questions = new List<IpipQuestion>
    {
        new IpipQuestion { ItemNumber = 1, Text = "Worry about things.", Trait = "N", Facet = "N1", IsReverse = false },
        new IpipQuestion { ItemNumber = 2, Text = "Get stressed out easily.", Trait = "N", Facet = "N1", IsReverse = false },
        new IpipQuestion { ItemNumber = 3, Text = "Get angry easily.", Trait = "N", Facet = "N2", IsReverse = false },
        new IpipQuestion { ItemNumber = 4, Text = "Lose my temper.", Trait = "N", Facet = "N2", IsReverse = false },
        new IpipQuestion { ItemNumber = 5, Text = "Often feel blue.", Trait = "N", Facet = "N3", IsReverse = false },
        new IpipQuestion { ItemNumber = 6, Text = "Dislike myself.", Trait = "N", Facet = "N3", IsReverse = false },
        new IpipQuestion { ItemNumber = 7, Text = "Find it difficult to approach others.", Trait = "N", Facet = "N4", IsReverse = false },
        new IpipQuestion { ItemNumber = 8, Text = "Am easily intimidated.", Trait = "N", Facet = "N4", IsReverse = false },
        new IpipQuestion { ItemNumber = 9, Text = "Rarely overindulge.", Trait = "N", Facet = "N5", IsReverse = true },
        new IpipQuestion { ItemNumber = 10, Text = "Am able to control my cravings.", Trait = "N", Facet = "N5", IsReverse = true },
        new IpipQuestion { ItemNumber = 11, Text = "Remain calm under pressure.", Trait = "N", Facet = "N6", IsReverse = true },
        new IpipQuestion { ItemNumber = 12, Text = "Am calm even in tense situations.", Trait = "N", Facet = "N6", IsReverse = true },
        
        new IpipQuestion { ItemNumber = 13, Text = "Make friends easily.", Trait = "E", Facet = "E1", IsReverse = false },
        new IpipQuestion { ItemNumber = 14, Text = "Act comfortably with others.", Trait = "E", Facet = "E1", IsReverse = false },
        new IpipQuestion { ItemNumber = 15, Text = "Love large parties.", Trait = "E", Facet = "E2", IsReverse = false },
        new IpipQuestion { ItemNumber = 16, Text = "Avoid crowds.", Trait = "E", Facet = "E2", IsReverse = true },
        new IpipQuestion { ItemNumber = 17, Text = "Take charge.", Trait = "E", Facet = "E3", IsReverse = false },
        new IpipQuestion { ItemNumber = 18, Text = "Try to lead others.", Trait = "E", Facet = "E3", IsReverse = false },
        new IpipQuestion { ItemNumber = 19, Text = "Am always busy.", Trait = "E", Facet = "E4", IsReverse = false },
        new IpipQuestion { ItemNumber = 20, Text = "Am always on the go.", Trait = "E", Facet = "E4", IsReverse = false },
        new IpipQuestion { ItemNumber = 21, Text = "Love excitement.", Trait = "E", Facet = "E5", IsReverse = false },
        new IpipQuestion { ItemNumber = 22, Text = "Seek adventure.", Trait = "E", Facet = "E5", IsReverse = false },
        new IpipQuestion { ItemNumber = 23, Text = "Have a lot of fun.", Trait = "E", Facet = "E6", IsReverse = false },
        new IpipQuestion { ItemNumber = 24, Text = "Love life.", Trait = "E", Facet = "E6", IsReverse = false },
        
        new IpipQuestion { ItemNumber = 25, Text = "Have a vivid imagination.", Trait = "O", Facet = "O1", IsReverse = false },
        new IpipQuestion { ItemNumber = 26, Text = "Love to daydream.", Trait = "O", Facet = "O1", IsReverse = false },
        new IpipQuestion { ItemNumber = 27, Text = "Believe in the importance of art.", Trait = "O", Facet = "O2", IsReverse = false },
        new IpipQuestion { ItemNumber = 28, Text = "Do not like art.", Trait = "O", Facet = "O2", IsReverse = true },
        new IpipQuestion { ItemNumber = 29, Text = "Experience my emotions intensely.", Trait = "O", Facet = "O3", IsReverse = false },
        new IpipQuestion { ItemNumber = 30, Text = "Am not easily affected by my emotions.", Trait = "O", Facet = "O3", IsReverse = true },
        new IpipQuestion { ItemNumber = 31, Text = "Prefer to stick with things that I know.", Trait = "O", Facet = "O4", IsReverse = true },
        new IpipQuestion { ItemNumber = 32, Text = "Don’t like the idea of change.", Trait = "O", Facet = "O4", IsReverse = true },
        new IpipQuestion { ItemNumber = 33, Text = "Avoid philosophical discussions.", Trait = "O", Facet = "O5", IsReverse = true },
        new IpipQuestion { ItemNumber = 34, Text = "Am not interested in theoretical discussions.", Trait = "O", Facet = "O5", IsReverse = true },
        new IpipQuestion { ItemNumber = 35, Text = "Tend to vote for liberal political candidates.", Trait = "O", Facet = "O6", IsReverse = false },
        new IpipQuestion { ItemNumber = 36, Text = "Believe in one true religion.", Trait = "O", Facet = "O6", IsReverse = true },
        
        new IpipQuestion { ItemNumber = 37, Text = "Trust others.", Trait = "A", Facet = "A1", IsReverse = false },
        new IpipQuestion { ItemNumber = 38, Text = "Believe that others have good intentions.", Trait = "A", Facet = "A1", IsReverse = false },
        new IpipQuestion { ItemNumber = 39, Text = "Cheat to get ahead.", Trait = "A", Facet = "A2", IsReverse = true },
        new IpipQuestion { ItemNumber = 40, Text = "Take advantage of others.", Trait = "A", Facet = "A2", IsReverse = true },
        new IpipQuestion { ItemNumber = 41, Text = "Love to help others.", Trait = "A", Facet = "A3", IsReverse = false },
        new IpipQuestion { ItemNumber = 42, Text = "Am concerned about others.", Trait = "A", Facet = "A3", IsReverse = false },
        new IpipQuestion { ItemNumber = 43, Text = "Insult people.", Trait = "A", Facet = "A4", IsReverse = true },
        new IpipQuestion { ItemNumber = 44, Text = "Get back at others.", Trait = "A", Facet = "A4", IsReverse = true },
        new IpipQuestion { ItemNumber = 45, Text = "Believe that I am better than others.", Trait = "A", Facet = "A5", IsReverse = true },
        new IpipQuestion { ItemNumber = 46, Text = "Think highly of myself.", Trait = "A", Facet = "A5", IsReverse = true },
        new IpipQuestion { ItemNumber = 47, Text = "Sympathize with the homeless.", Trait = "A", Facet = "A6", IsReverse = false },
        new IpipQuestion { ItemNumber = 48, Text = "Feel sympathy for those who are worse off than myself.", Trait = "A", Facet = "A6", IsReverse = false },
        
        new IpipQuestion { ItemNumber = 49, Text = "Handle tasks smoothly.", Trait = "C", Facet = "C1", IsReverse = false },
        new IpipQuestion { ItemNumber = 50, Text = "Know how to get things done.", Trait = "C", Facet = "C1", IsReverse = false },
        new IpipQuestion { ItemNumber = 51, Text = "Like to tidy up.", Trait = "C", Facet = "C2", IsReverse = false },
        new IpipQuestion { ItemNumber = 52, Text = "Leave a mess in my room.", Trait = "C", Facet = "C2", IsReverse = true },
        new IpipQuestion { ItemNumber = 53, Text = "Tell the truth.", Trait = "C", Facet = "C3", IsReverse = false },
        new IpipQuestion { ItemNumber = 54, Text = "Break my promises.", Trait = "C", Facet = "C3", IsReverse = true },
        new IpipQuestion { ItemNumber = 55, Text = "Work hard.", Trait = "C", Facet = "C4", IsReverse = false },
        new IpipQuestion { ItemNumber = 56, Text = "Set high standards for myself and others.", Trait = "C", Facet = "C4", IsReverse = false },
        new IpipQuestion { ItemNumber = 57, Text = "Carry out my plans.", Trait = "C", Facet = "C5", IsReverse = false },
        new IpipQuestion { ItemNumber = 58, Text = "Have difficulty starting tasks.", Trait = "C", Facet = "C5", IsReverse = true },
        new IpipQuestion { ItemNumber = 59, Text = "Make rash decisions.", Trait = "C", Facet = "C6", IsReverse = true },
        new IpipQuestion { ItemNumber = 60, Text = "Act without thinking.", Trait = "C", Facet = "C6", IsReverse = true }
    };
}
