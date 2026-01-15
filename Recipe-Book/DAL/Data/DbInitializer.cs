using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(RecipeDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Ingredients.AnyAsync())
        {
            return;
        }

        await SeedAllergensAsync(context);
        await SeedCategoriesAsync(context);
        await SeedIngredientsAsync(context);
        await SeedRecipesAsync(context);
    }

    private static async Task SeedAllergensAsync(RecipeDbContext context)
    {
        var allergens = new[]
        {
            new Allergen { Name = "Gluten", Description = "Found in wheat, barley, rye" },
            new Allergen { Name = "Dairy", Description = "Milk and milk products" },
            new Allergen { Name = "Eggs", Description = "Eggs and egg products" },
            new Allergen { Name = "Tree Nuts", Description = "Almonds, walnuts, cashews, etc." },
            new Allergen { Name = "Peanuts", Description = "Peanuts and peanut products" },
            new Allergen { Name = "Soy", Description = "Soybeans and soy products" },
            new Allergen { Name = "Shellfish", Description = "Shrimp, crab, lobster, etc." },
            new Allergen { Name = "Fish", Description = "All fish species" },
            new Allergen { Name = "Sesame", Description = "Sesame seeds and tahini" }
        };

        context.Allergens.AddRange(allergens);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(RecipeDbContext context)
    {
        var categories = new[]
        {
            new Category { Name = "Breakfast", Description = "Morning meals" },
            new Category { Name = "Lunch", Description = "Midday meals" },
            new Category { Name = "Dinner", Description = "Evening meals" },
            new Category { Name = "Dessert", Description = "Sweet treats" },
            new Category { Name = "Snack", Description = "Light bites" },
            new Category { Name = "Appetizer", Description = "Starters" },
            new Category { Name = "Soup", Description = "Hot liquids" },
            new Category { Name = "Salad", Description = "Cold vegetable dishes" }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedIngredientsAsync(RecipeDbContext context)
    {
        var gluten = await context.Allergens.FirstAsync(a => a.Name == "Gluten");
        var dairy = await context.Allergens.FirstAsync(a => a.Name == "Dairy");
        var eggs = await context.Allergens.FirstAsync(a => a.Name == "Eggs");
        var treeNuts = await context.Allergens.FirstAsync(a => a.Name == "Tree Nuts");
        var fish = await context.Allergens.FirstAsync(a => a.Name == "Fish");
        var shellfish = await context.Allergens.FirstAsync(a => a.Name == "Shellfish");
        var soy = await context.Allergens.FirstAsync(a => a.Name == "Soy");

        var ingredients = new List<Ingredient>
        {
            new Ingredient
            {
                Name = "All-Purpose Flour",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Grains,
                IsVegetarian = true,
                IsVegan = true,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = gluten }
                }
            },
            new Ingredient
            {
                Name = "Rice",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Grains,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Pasta",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Grains,
                IsVegetarian = true,
                IsVegan = true,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = gluten }
                }
            },
            new Ingredient
            {
                Name = "Oats",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Grains,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Bread",
                DefaultUnit = MeasurementUnit.Pieces,
                Category = IngredientCategory.Grains,
                IsVegetarian = true,
                IsVegan = false,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = gluten }
                }
            },
            new Ingredient
            {
                Name = "Chicken Breast",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Poultry,
                IsVegetarian = false,
                IsVegan = false
            },
            new Ingredient
            {
                Name = "Beef",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Meat,
                IsVegetarian = false,
                IsVegan = false
            },
            new Ingredient
            {
                Name = "Salmon",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Fish,
                IsVegetarian = false,
                IsVegan = false,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = fish }
                }
            },
            new Ingredient
            {
                Name = "Shrimp",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Seafood,
                IsVegetarian = false,
                IsVegan = false,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = shellfish }
                }
            },
            new Ingredient
            {
                Name = "Tofu",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Legumes,
                IsVegetarian = true,
                IsVegan = true,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = soy }
                }
            },
            new Ingredient
            {
                Name = "Milk",
                DefaultUnit = MeasurementUnit.Milliliters,
                Category = IngredientCategory.Dairy,
                IsVegetarian = true,
                IsVegan = false,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = dairy }
                }
            },
            new Ingredient
            {
                Name = "Butter",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Dairy,
                IsVegetarian = true,
                IsVegan = false,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = dairy }
                }
            },
            new Ingredient
            {
                Name = "Cheese",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Dairy,
                IsVegetarian = true,
                IsVegan = false,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = dairy }
                }
            },
            new Ingredient
            {
                Name = "Yogurt",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Dairy,
                IsVegetarian = true,
                IsVegan = false,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = dairy }
                }
            },
            new Ingredient
            {
                Name = "Eggs",
                DefaultUnit = MeasurementUnit.Whole,
                Category = IngredientCategory.Eggs,
                IsVegetarian = true,
                IsVegan = false,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = eggs }
                }
            },
            new Ingredient
            {
                Name = "Tomato",
                DefaultUnit = MeasurementUnit.Whole,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Onion",
                DefaultUnit = MeasurementUnit.Whole,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Garlic",
                DefaultUnit = MeasurementUnit.Whole,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Carrot",
                DefaultUnit = MeasurementUnit.Whole,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Bell Pepper",
                DefaultUnit = MeasurementUnit.Whole,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Spinach",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Broccoli",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Potato",
                DefaultUnit = MeasurementUnit.Whole,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Mushrooms",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Lettuce",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Vegetables,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Olive Oil",
                DefaultUnit = MeasurementUnit.Milliliters,
                Category = IngredientCategory.Oils,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Salt",
                DefaultUnit = MeasurementUnit.Teaspoons,
                Category = IngredientCategory.Spices,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Black Pepper",
                DefaultUnit = MeasurementUnit.Teaspoons,
                Category = IngredientCategory.Spices,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Sugar",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Sweeteners,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Baking Powder",
                DefaultUnit = MeasurementUnit.Teaspoons,
                Category = IngredientCategory.Baking,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Vanilla Extract",
                DefaultUnit = MeasurementUnit.Teaspoons,
                Category = IngredientCategory.Baking,
                IsVegetarian = true,
                IsVegan = true
            },
            new Ingredient
            {
                Name = "Soy Sauce",
                DefaultUnit = MeasurementUnit.Tablespoons,
                Category = IngredientCategory.Condiments,
                IsVegetarian = true,
                IsVegan = true,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = soy },
                    new IngredientAllergen { Allergen = gluten }
                }
            },
            new Ingredient
            {
                Name = "Almonds",
                DefaultUnit = MeasurementUnit.Grams,
                Category = IngredientCategory.Nuts,
                IsVegetarian = true,
                IsVegan = true,
                IngredientAllergens = new List<IngredientAllergen>
                {
                    new IngredientAllergen { Allergen = treeNuts }
                }
            }
        };

        context.Ingredients.AddRange(ingredients);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRecipesAsync(RecipeDbContext context)
    {
        var breakfast = await context.Categories.FirstAsync(c => c.Name == "Breakfast");
        var lunch = await context.Categories.FirstAsync(c => c.Name == "Lunch");
        var dinner = await context.Categories.FirstAsync(c => c.Name == "Dinner");
        var dessert = await context.Categories.FirstAsync(c => c.Name == "Dessert");
        var soup = await context.Categories.FirstAsync(c => c.Name == "Soup");
        var salad = await context.Categories.FirstAsync(c => c.Name == "Salad");

        var flour = await context.Ingredients.FirstAsync(i => i.Name == "All-Purpose Flour");
        var milk = await context.Ingredients.FirstAsync(i => i.Name == "Milk");
        var eggs = await context.Ingredients.FirstAsync(i => i.Name == "Eggs");
        var butter = await context.Ingredients.FirstAsync(i => i.Name == "Butter");
        var sugar = await context.Ingredients.FirstAsync(i => i.Name == "Sugar");
        var bakingPowder = await context.Ingredients.FirstAsync(i => i.Name == "Baking Powder");
        var vanilla = await context.Ingredients.FirstAsync(i => i.Name == "Vanilla Extract");
        var chicken = await context.Ingredients.FirstAsync(i => i.Name == "Chicken Breast");
        var rice = await context.Ingredients.FirstAsync(i => i.Name == "Rice");
        var pasta = await context.Ingredients.FirstAsync(i => i.Name == "Pasta");
        var tomato = await context.Ingredients.FirstAsync(i => i.Name == "Tomato");
        var onion = await context.Ingredients.FirstAsync(i => i.Name == "Onion");
        var garlic = await context.Ingredients.FirstAsync(i => i.Name == "Garlic");
        var cheese = await context.Ingredients.FirstAsync(i => i.Name == "Cheese");
        var oliveOil = await context.Ingredients.FirstAsync(i => i.Name == "Olive Oil");
        var salt = await context.Ingredients.FirstAsync(i => i.Name == "Salt");
        var pepper = await context.Ingredients.FirstAsync(i => i.Name == "Black Pepper");
        var lettuce = await context.Ingredients.FirstAsync(i => i.Name == "Lettuce");
        var carrot = await context.Ingredients.FirstAsync(i => i.Name == "Carrot");
        var potato = await context.Ingredients.FirstAsync(i => i.Name == "Potato");
        var tofu = await context.Ingredients.FirstAsync(i => i.Name == "Tofu");
        var soySauce = await context.Ingredients.FirstAsync(i => i.Name == "Soy Sauce");
        var bellPepper = await context.Ingredients.FirstAsync(i => i.Name == "Bell Pepper");
        var spinach = await context.Ingredients.FirstAsync(i => i.Name == "Spinach");
        var mushrooms = await context.Ingredients.FirstAsync(i => i.Name == "Mushrooms");
        var broccoli = await context.Ingredients.FirstAsync(i => i.Name == "Broccoli");
        var yogurt = await context.Ingredients.FirstAsync(i => i.Name == "Yogurt");
        var oats = await context.Ingredients.FirstAsync(i => i.Name == "Oats");
        var salmon = await context.Ingredients.FirstAsync(i => i.Name == "Salmon");
        var beef = await context.Ingredients.FirstAsync(i => i.Name == "Beef");
        var shrimp = await context.Ingredients.FirstAsync(i => i.Name == "Shrimp");
        var bread = await context.Ingredients.FirstAsync(i => i.Name == "Bread");
        var almonds = await context.Ingredients.FirstAsync(i => i.Name == "Almonds");

        var recipes = new List<Recipe>
        {
            new Recipe
            {
                Name = "Classic Pancakes",
                Description = "Fluffy American-style pancakes perfect for breakfast.\n\nLight, airy, and delicious with maple syrup!",
                BaseServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 15,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = breakfast }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = flour, Amount = 200, Unit = MeasurementUnit.Grams, SortOrder = 1 },
                    new RecipeIngredient { Ingredient = milk, Amount = 300, Unit = MeasurementUnit.Milliliters, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = eggs, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 3 },
                    new RecipeIngredient { Ingredient = butter, Amount = 50, Unit = MeasurementUnit.Grams, SortOrder = 4, PrepNote = "melted" },
                    new RecipeIngredient { Ingredient = sugar, Amount = 30, Unit = MeasurementUnit.Grams, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = bakingPowder, Amount = 2, Unit = MeasurementUnit.Teaspoons, SortOrder = 6 },
                    new RecipeIngredient { Ingredient = vanilla, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 7, IsOptional = true }
                }
            },
            new Recipe
            {
                Name = "Scrambled Eggs",
                Description = "Simple and delicious scrambled eggs for a quick breakfast.",
                BaseServingSize = 2,
                PrepTimeMinutes = 2,
                CookTimeMinutes = 5,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = breakfast }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = eggs, Amount = 4, Unit = MeasurementUnit.Whole, SortOrder = 1 },
                    new RecipeIngredient { Ingredient = butter, Amount = 20, Unit = MeasurementUnit.Grams, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = milk, Amount = 50, Unit = MeasurementUnit.Milliliters, SortOrder = 3 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Pinch, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Pinch, SortOrder = 5 }
                }
            },
            new Recipe
            {
                Name = "Spaghetti Carbonara",
                Description = "Classic Italian pasta with creamy egg and cheese sauce.",
                BaseServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 20,
                DifficultyLevel = DifficultyLevel.Medium,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dinner }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = pasta, Amount = 400, Unit = MeasurementUnit.Grams, SortOrder = 1 },
                    new RecipeIngredient { Ingredient = eggs, Amount = 4, Unit = MeasurementUnit.Whole, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = cheese, Amount = 100, Unit = MeasurementUnit.Grams, SortOrder = 3, PrepNote = "grated parmesan" },
                    new RecipeIngredient { Ingredient = garlic, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 4, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 6 }
                }
            },
            new Recipe
            {
                Name = "Grilled Chicken with Rice",
                Description = "Healthy grilled chicken breast served with fluffy white rice.",
                BaseServingSize = 2,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 30,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = lunch },
                    new RecipeCategory { Category = dinner }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = chicken, Amount = 400, Unit = MeasurementUnit.Grams, SortOrder = 1 },
                    new RecipeIngredient { Ingredient = rice, Amount = 200, Unit = MeasurementUnit.Grams, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 30, Unit = MeasurementUnit.Milliliters, SortOrder = 3 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 5 }
                }
            },
            new Recipe
            {
                Name = "Caesar Salad",
                Description = "Fresh and crispy Caesar salad with homemade dressing.",
                BaseServingSize = 4,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 0,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = salad },
                    new RecipeCategory { Category = lunch }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = lettuce, Amount = 300, Unit = MeasurementUnit.Grams, SortOrder = 1, PrepNote = "chopped" },
                    new RecipeIngredient { Ingredient = cheese, Amount = 50, Unit = MeasurementUnit.Grams, SortOrder = 2, PrepNote = "shaved parmesan" },
                    new RecipeIngredient { Ingredient = eggs, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 3, PrepNote = "hard boiled" },
                    new RecipeIngredient { Ingredient = garlic, Amount = 1, Unit = MeasurementUnit.Whole, SortOrder = 4, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 60, Unit = MeasurementUnit.Milliliters, SortOrder = 5 }
                }
            },
            new Recipe
            {
                Name = "Tomato Soup",
                Description = "Creamy tomato soup perfect for a cold day.",
                BaseServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 30,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = soup }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = tomato, Amount = 8, Unit = MeasurementUnit.Whole, SortOrder = 1, PrepNote = "diced" },
                    new RecipeIngredient { Ingredient = onion, Amount = 1, Unit = MeasurementUnit.Whole, SortOrder = 2, PrepNote = "diced" },
                    new RecipeIngredient { Ingredient = garlic, Amount = 3, Unit = MeasurementUnit.Whole, SortOrder = 3, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 30, Unit = MeasurementUnit.Milliliters, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 6 }
                }
            },
            new Recipe
            {
                Name = "Vegetable Stir-Fry",
                Description = "Quick and healthy vegetable stir-fry with tofu.",
                BaseServingSize = 2,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 10,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dinner }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = tofu, Amount = 200, Unit = MeasurementUnit.Grams, SortOrder = 1, PrepNote = "cubed" },
                    new RecipeIngredient { Ingredient = bellPepper, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 2, PrepNote = "sliced" },
                    new RecipeIngredient { Ingredient = carrot, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 3, PrepNote = "sliced" },
                    new RecipeIngredient { Ingredient = onion, Amount = 1, Unit = MeasurementUnit.Whole, SortOrder = 4, PrepNote = "sliced" },
                    new RecipeIngredient { Ingredient = garlic, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 5, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = soySauce, Amount = 3, Unit = MeasurementUnit.Tablespoons, SortOrder = 6 },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 30, Unit = MeasurementUnit.Milliliters, SortOrder = 7 }
                }
            },
            new Recipe
            {
                Name = "Mashed Potatoes",
                Description = "Creamy and buttery mashed potatoes.",
                BaseServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 20,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dinner }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = potato, Amount = 6, Unit = MeasurementUnit.Whole, SortOrder = 1, PrepNote = "peeled" },
                    new RecipeIngredient { Ingredient = butter, Amount = 80, Unit = MeasurementUnit.Grams, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = milk, Amount = 100, Unit = MeasurementUnit.Milliliters, SortOrder = 3 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Pinch, SortOrder = 5 }
                }
            },
            new Recipe
            {
                Name = "Simple Vanilla Cake",
                Description = "Moist vanilla cake perfect for any celebration.",
                BaseServingSize = 8,
                PrepTimeMinutes = 20,
                CookTimeMinutes = 35,
                DifficultyLevel = DifficultyLevel.Medium,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dessert }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = flour, Amount = 300, Unit = MeasurementUnit.Grams, SortOrder = 1 },
                    new RecipeIngredient { Ingredient = sugar, Amount = 250, Unit = MeasurementUnit.Grams, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = butter, Amount = 150, Unit = MeasurementUnit.Grams, SortOrder = 3, PrepNote = "softened" },
                    new RecipeIngredient { Ingredient = eggs, Amount = 4, Unit = MeasurementUnit.Whole, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = milk, Amount = 200, Unit = MeasurementUnit.Milliliters, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = bakingPowder, Amount = 3, Unit = MeasurementUnit.Teaspoons, SortOrder = 6 },
                    new RecipeIngredient { Ingredient = vanilla, Amount = 2, Unit = MeasurementUnit.Teaspoons, SortOrder = 7 }
                }
            },
            new Recipe
            {
                Name = "Garlic Butter Chicken",
                Description = "Juicy chicken breast with a rich garlic butter sauce.",
                BaseServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 25,
                DifficultyLevel = DifficultyLevel.Medium,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dinner }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = chicken, Amount = 600, Unit = MeasurementUnit.Grams, SortOrder = 1 },
                    new RecipeIngredient { Ingredient = butter, Amount = 60, Unit = MeasurementUnit.Grams, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = garlic, Amount = 6, Unit = MeasurementUnit.Whole, SortOrder = 3, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 30, Unit = MeasurementUnit.Milliliters, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 6 }
                }
            },
            new Recipe
            {
                Name = "Overnight Oats",
                Description = "Healthy breakfast prepared the night before.\n\nPacked with fiber and nutrition for a great start to your day!",
                BaseServingSize = 2,
                PrepTimeMinutes = 5,
                CookTimeMinutes = 0,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = breakfast }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = oats, Amount = 200, Unit = MeasurementUnit.Grams, SortOrder = 1 },
                    new RecipeIngredient { Ingredient = yogurt, Amount = 250, Unit = MeasurementUnit.Grams, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = milk, Amount = 200, Unit = MeasurementUnit.Milliliters, SortOrder = 3 },
                    new RecipeIngredient { Ingredient = sugar, Amount = 20, Unit = MeasurementUnit.Grams, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = vanilla, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 5, IsOptional = true }
                }
            },
            new Recipe
            {
                Name = "Baked Salmon with Vegetables",
                Description = "Delicious and nutritious baked salmon with roasted vegetables.\n\nRich in omega-3 fatty acids and vitamins!",
                BaseServingSize = 4,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 25,
                DifficultyLevel = DifficultyLevel.Medium,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dinner },
                    new RecipeCategory { Category = lunch }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = salmon, Amount = 600, Unit = MeasurementUnit.Grams, SortOrder = 1, PrepNote = "fillets" },
                    new RecipeIngredient { Ingredient = broccoli, Amount = 300, Unit = MeasurementUnit.Grams, SortOrder = 2, PrepNote = "florets" },
                    new RecipeIngredient { Ingredient = carrot, Amount = 3, Unit = MeasurementUnit.Whole, SortOrder = 3, PrepNote = "sliced" },
                    new RecipeIngredient { Ingredient = bellPepper, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 4, PrepNote = "chopped" },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 60, Unit = MeasurementUnit.Milliliters, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = garlic, Amount = 3, Unit = MeasurementUnit.Whole, SortOrder = 6, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 7 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 8 }
                }
            },
            new Recipe
            {
                Name = "Classic Beef Burger",
                Description = "Juicy homemade burger with all the fixings.\n\nPerfect for summer barbecues!",
                BaseServingSize = 4,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 10,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = lunch },
                    new RecipeCategory { Category = dinner }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = beef, Amount = 600, Unit = MeasurementUnit.Grams, SortOrder = 1, PrepNote = "ground" },
                    new RecipeIngredient { Ingredient = bread, Amount = 4, Unit = MeasurementUnit.Pieces, SortOrder = 2, PrepNote = "burger buns" },
                    new RecipeIngredient { Ingredient = cheese, Amount = 100, Unit = MeasurementUnit.Grams, SortOrder = 3, PrepNote = "sliced" },
                    new RecipeIngredient { Ingredient = lettuce, Amount = 100, Unit = MeasurementUnit.Grams, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = tomato, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 5, PrepNote = "sliced" },
                    new RecipeIngredient { Ingredient = onion, Amount = 1, Unit = MeasurementUnit.Whole, SortOrder = 6, PrepNote = "sliced" },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 7 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 8 }
                }
            },
            new Recipe
            {
                Name = "Shrimp Scampi",
                Description = "Garlicky shrimp in white wine butter sauce.\n\nElegant and restaurant-quality dish!",
                BaseServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 15,
                DifficultyLevel = DifficultyLevel.Medium,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dinner }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = shrimp, Amount = 500, Unit = MeasurementUnit.Grams, SortOrder = 1, PrepNote = "peeled and deveined" },
                    new RecipeIngredient { Ingredient = pasta, Amount = 400, Unit = MeasurementUnit.Grams, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = butter, Amount = 80, Unit = MeasurementUnit.Grams, SortOrder = 3 },
                    new RecipeIngredient { Ingredient = garlic, Amount = 6, Unit = MeasurementUnit.Whole, SortOrder = 4, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 30, Unit = MeasurementUnit.Milliliters, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 6 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 7 }
                }
            },
            new Recipe
            {
                Name = "French Toast",
                Description = "Classic crispy and custardy French toast.\n\nPerfect weekend breakfast!",
                BaseServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 15,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = breakfast }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = bread, Amount = 8, Unit = MeasurementUnit.Pieces, SortOrder = 1, PrepNote = "thick slices" },
                    new RecipeIngredient { Ingredient = eggs, Amount = 4, Unit = MeasurementUnit.Whole, SortOrder = 2 },
                    new RecipeIngredient { Ingredient = milk, Amount = 200, Unit = MeasurementUnit.Milliliters, SortOrder = 3 },
                    new RecipeIngredient { Ingredient = sugar, Amount = 30, Unit = MeasurementUnit.Grams, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = vanilla, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = butter, Amount = 40, Unit = MeasurementUnit.Grams, SortOrder = 6, PrepNote = "for cooking" }
                }
            },
            new Recipe
            {
                Name = "Mushroom Risotto",
                Description = "Creamy Italian risotto with earthy mushrooms.\n\nComfort food at its finest!",
                BaseServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 35,
                DifficultyLevel = DifficultyLevel.Hard,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dinner }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = rice, Amount = 320, Unit = MeasurementUnit.Grams, SortOrder = 1, PrepNote = "arborio rice" },
                    new RecipeIngredient { Ingredient = mushrooms, Amount = 400, Unit = MeasurementUnit.Grams, SortOrder = 2, PrepNote = "sliced" },
                    new RecipeIngredient { Ingredient = onion, Amount = 1, Unit = MeasurementUnit.Whole, SortOrder = 3, PrepNote = "diced" },
                    new RecipeIngredient { Ingredient = garlic, Amount = 3, Unit = MeasurementUnit.Whole, SortOrder = 4, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = butter, Amount = 60, Unit = MeasurementUnit.Grams, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = cheese, Amount = 80, Unit = MeasurementUnit.Grams, SortOrder = 6, PrepNote = "parmesan" },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 30, Unit = MeasurementUnit.Milliliters, SortOrder = 7 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 8 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 9 }
                }
            },
            new Recipe
            {
                Name = "Greek Salad",
                Description = "Fresh and vibrant Mediterranean salad.\n\nHealthy and bursting with flavor!",
                BaseServingSize = 4,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 0,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = salad },
                    new RecipeCategory { Category = lunch }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = tomato, Amount = 4, Unit = MeasurementUnit.Whole, SortOrder = 1, PrepNote = "chopped" },
                    new RecipeIngredient { Ingredient = lettuce, Amount = 200, Unit = MeasurementUnit.Grams, SortOrder = 2, PrepNote = "chopped" },
                    new RecipeIngredient { Ingredient = onion, Amount = 1, Unit = MeasurementUnit.Whole, SortOrder = 3, PrepNote = "sliced" },
                    new RecipeIngredient { Ingredient = cheese, Amount = 150, Unit = MeasurementUnit.Grams, SortOrder = 4, PrepNote = "feta cheese" },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 60, Unit = MeasurementUnit.Milliliters, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 6 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Pinch, SortOrder = 7 }
                }
            },
            new Recipe
            {
                Name = "Vegetarian Chili",
                Description = "Hearty and spicy vegetarian chili.\n\nPacked with protein and flavor!",
                BaseServingSize = 6,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 45,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dinner },
                    new RecipeCategory { Category = soup }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = tomato, Amount = 6, Unit = MeasurementUnit.Whole, SortOrder = 1, PrepNote = "diced" },
                    new RecipeIngredient { Ingredient = onion, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 2, PrepNote = "diced" },
                    new RecipeIngredient { Ingredient = garlic, Amount = 4, Unit = MeasurementUnit.Whole, SortOrder = 3, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = bellPepper, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 4, PrepNote = "diced" },
                    new RecipeIngredient { Ingredient = carrot, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 5, PrepNote = "diced" },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 30, Unit = MeasurementUnit.Milliliters, SortOrder = 6 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 7 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 8 }
                }
            },
            new Recipe
            {
                Name = "Garlic Bread",
                Description = "Crispy and buttery garlic bread.\n\nPerfect side for pasta dishes!",
                BaseServingSize = 6,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 12,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = breakfast }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = bread, Amount = 1, Unit = MeasurementUnit.Whole, SortOrder = 1, PrepNote = "baguette" },
                    new RecipeIngredient { Ingredient = butter, Amount = 100, Unit = MeasurementUnit.Grams, SortOrder = 2, PrepNote = "softened" },
                    new RecipeIngredient { Ingredient = garlic, Amount = 6, Unit = MeasurementUnit.Whole, SortOrder = 3, PrepNote = "minced" },
                    new RecipeIngredient { Ingredient = cheese, Amount = 50, Unit = MeasurementUnit.Grams, SortOrder = 4, PrepNote = "parmesan", IsOptional = true },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Pinch, SortOrder = 5 }
                }
            },
            new Recipe
            {
                Name = "Spinach and Cheese Omelette",
                Description = "Fluffy omelette filled with spinach and cheese.\n\nQuick and nutritious breakfast!",
                BaseServingSize = 2,
                PrepTimeMinutes = 5,
                CookTimeMinutes = 8,
                DifficultyLevel = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = breakfast }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = eggs, Amount = 6, Unit = MeasurementUnit.Whole, SortOrder = 1 },
                    new RecipeIngredient { Ingredient = spinach, Amount = 150, Unit = MeasurementUnit.Grams, SortOrder = 2, PrepNote = "fresh" },
                    new RecipeIngredient { Ingredient = cheese, Amount = 80, Unit = MeasurementUnit.Grams, SortOrder = 3, PrepNote = "shredded" },
                    new RecipeIngredient { Ingredient = butter, Amount = 30, Unit = MeasurementUnit.Grams, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Pinch, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Pinch, SortOrder = 6 }
                }
            },
            new Recipe
            {
                Name = "Almond Crusted Chicken",
                Description = "Crunchy almond-crusted chicken breast.\n\nHealthy alternative to fried chicken!",
                BaseServingSize = 4,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 25,
                DifficultyLevel = DifficultyLevel.Medium,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                RecipeCategories = new List<RecipeCategory>
                {
                    new RecipeCategory { Category = dinner }
                },
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = chicken, Amount = 600, Unit = MeasurementUnit.Grams, SortOrder = 1 },
                    new RecipeIngredient { Ingredient = almonds, Amount = 150, Unit = MeasurementUnit.Grams, SortOrder = 2, PrepNote = "crushed" },
                    new RecipeIngredient { Ingredient = eggs, Amount = 2, Unit = MeasurementUnit.Whole, SortOrder = 3 },
                    new RecipeIngredient { Ingredient = flour, Amount = 100, Unit = MeasurementUnit.Grams, SortOrder = 4 },
                    new RecipeIngredient { Ingredient = oliveOil, Amount = 30, Unit = MeasurementUnit.Milliliters, SortOrder = 5 },
                    new RecipeIngredient { Ingredient = salt, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 6 },
                    new RecipeIngredient { Ingredient = pepper, Amount = 1, Unit = MeasurementUnit.Teaspoons, SortOrder = 7 }
                }
            }
        };

        context.Recipes.AddRange(recipes);
        await context.SaveChangesAsync();
    }
}
