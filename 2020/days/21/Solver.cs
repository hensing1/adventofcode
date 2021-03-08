using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._21
{
    [ProblemDate(21)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var ingredients = new HashSet<string>();
            var ingrAllergMap = new Dictionary<string, List<int>>();
            var foods = new List<(HashSet<string> Ingredients, List<int> Allergens)>();

            ReadInput(lines, out ingredients, out ingrAllergMap, out foods);

            foreach (var food in foods)
            {
                var allergFreeIngrs = new HashSet<string>(ingredients);
                allergFreeIngrs.ExceptWith(food.Ingredients);
                foreach (string ingredient in allergFreeIngrs)
                    foreach (int allergen in food.Allergens)
                        ingrAllergMap[ingredient].Remove(allergen);
            }

            string[] safeIngredients = ingredients.Where(ingredient => ingrAllergMap[ingredient].Count == 0).ToArray();

            int safeIngrOccurenceCount = 0;
            foreach (string ingredient in safeIngredients)
                safeIngrOccurenceCount += foods.Where(food => food.Ingredients.Contains(ingredient)).Count();

            return safeIngrOccurenceCount.ToString(); // 1st try boiiii
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var ingredients = new HashSet<string>();
            var ingrAllergMap = new Dictionary<string, List<int>>();
            var foods = new List<(HashSet<string> Ingredients, List<int> Allergens)>();
            var allergens = new List<string>();

            ReadInput(lines, out ingredients, out ingrAllergMap, out foods, out allergens);

            foreach (var food in foods)
            {
                var allergFreeIngrs = new HashSet<string>(ingredients);
                allergFreeIngrs.ExceptWith(food.Ingredients);
                foreach (string ingredient in allergFreeIngrs)
                    foreach (int allergen in food.Allergens)
                        ingrAllergMap[ingredient].Remove(allergen);
            };

            List<string> unsafeIngredients = ingredients.Where(ingredient => ingrAllergMap[ingredient].Count > 0).ToList();

            var ingrAllergMapFinal = new Dictionary<string, string>();
            while (unsafeIngredients.Count() > 0)
            {
                string solvedIngredient = unsafeIngredients.Single(ing => ingrAllergMap[ing].Count == 1);
                int solvedAllergIndex = ingrAllergMap[solvedIngredient].First();
                string solvedAllergen = allergens[solvedAllergIndex];

                unsafeIngredients.Remove(solvedIngredient);
                foreach (string ingredient in unsafeIngredients)
                    ingrAllergMap[ingredient].Remove(solvedAllergIndex);
                ingrAllergMapFinal.Add(solvedIngredient, solvedAllergen);
            }

            string[] orderedIngrs = ingrAllergMapFinal.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
            return String.Join(",", orderedIngrs);
        }

        private void ReadInput(string[] lines, out HashSet<string> ingredients, out Dictionary<string, List<int>> ingrAllergMap, out List<(HashSet<string>, List<int>)> foods)
        {
            var allergens = new List<string>();
            ReadInput(lines, out ingredients, out ingrAllergMap, out foods, out allergens);
        }
        private void ReadInput(string[] lines, out HashSet<string> ingredients, out Dictionary<string, List<int>> ingrAllergMap, 
                                out List<(HashSet<string>, List<int>)> foods, out List<string> allergens)
        {
            ingredients = new HashSet<string>();
            ingrAllergMap = new Dictionary<string, List<int>>();
            foods = new List<(HashSet<string> ingredients, List<int> allergens)>();
            allergens = new List<string>();

            foreach (string line in lines)
            {
                Match ingredientsMatch = Regex.Match(line, @"^(.+) \(");
                if (ingredientsMatch.Success)
                {
                    string[] foodIngredients = ingredientsMatch.Groups[1].Value.Split(' ');
                    foreach (string ingredient in foodIngredients)
                        if (!ingredients.Contains(ingredient))
                            ingredients.Add(ingredient);

                    Match allergensMatch = Regex.Match(line, @"\(contains (.+)\)");
                    if (allergensMatch.Success)
                    {
                        string[] foodAllergens = allergensMatch.Groups[1].Value.Split(new[] { ", " }, StringSplitOptions.None);
                        var allergenIndeces = new List<int>();
                        foreach (var allergen in foodAllergens)
                        {
                            if (!allergens.Contains(allergen))
                                allergens.Add(allergen);

                            allergenIndeces.Add(allergens.IndexOf(allergen));
                        }

                        foods.Add((foodIngredients.ToHashSet(), allergenIndeces));
                    }
                }
            }

            var allAllergenIndeces = Enumerable.Range(0, allergens.Count).ToList();
            foreach (string ingredient in ingredients)
                ingrAllergMap.Add(ingredient, new List<int>(allAllergenIndeces));
        }
    }
}
