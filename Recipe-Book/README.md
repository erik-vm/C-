# Programming Exam: Recipe Book Web Application – the AI Era

## Task Overview

Build a recipe management web application using ASP.NET Core Razor Pages. Your users are cooks who want to digitize their recipe collections, scale recipes for different group sizes, and find recipes based on available ingredients while avoiding allergens.

This is an open-book exam. Use **AI assistants**, documentation, whatever you need. We evaluate the final product - architecture decisions, domain modeling, validation quality, UX sensibility, edge case handling and **explanations about your system understanding and design**.

**Time limit:** 8 hours

**Technology stack:** ASP.NET Core Razor Pages, Entity Framework Core, SQLite.

**Hard requirements:** Enable nullable reference type errors, layered approach (DAL, BLL, UI).

**AI rules:** Spec driven approach strongly recommended. Add and save all the human written prompts to code repository.

## The Problem

Your client runs cooking classes and manages a large recipe collection. They've described their needs:

"I have hundreds of recipes written for different serving sizes—some feed 2, others feed 12. When I teach a class of varying size, I waste time manually recalculating every ingredient amount. I need the system to do this automatically and intelligently—nobody measures 0.37 teaspoons.

Many of my students have food allergies or dietary restrictions. I constantly worry about accidentally suggesting a recipe with hidden allergens. Sesame in tahini. Dairy in some breads. Fish sauce in Pad Thai. I need to flag these at the ingredient level and filter recipes accordingly.

I also want students to tell me what's in their fridge, and the app suggests what they can make. Doesn't have to be a perfect match—if they're missing one or two things, still show the recipe and tell them what to buy."

## Functional Requirements

### Recipe Management

Recipes have names, descriptions, and a base serving size (the number of portions the original recipe makes). Track preparation and cooking times separately so users can filter by total time. Assign difficulty levels for filtering. Recipes can belong to multiple meal categories (breakfast, dinner, dessert, etc.). Descriptions should support basic text formatting for readability. Use markdown, convert it to html for display. Track when recipes are created and modified.

### Ingredient Library

Maintain a master list of ingredients independent of recipes. Each ingredient has a default unit of measurement (grams, cups, pieces, etc.) and belongs to a category (meat, dairy, vegetable, spice, etc.).

Ingredient names must be unique across the system. When displaying or searching, treat "Tomato" and "tomato" as identical.

### Allergen & Dietary Classification

**This is critical for user safety.**

Ingredients must be tagged with allergen info: gluten, dairy, eggs, tree nuts, peanuts, soy, shellfish, fish, sesame, etc. An ingredient can have multiple allergen tags (e.g., some bread contains both gluten and dairy). Also track dietary flags: whether an ingredient is vegetarian, whether it's vegan. These must be logically consistent—vegan items are always vegetarian, anything containing dairy or eggs cannot be vegan, meat and fish cannot be vegetarian.

### Recipe Ingredients

When adding an ingredient to a recipe, specify the amount and unit. The unit can differ from the ingredient's default (flour might default to grams, but a recipe might specify cups). Mark ingredients as optional when they're not essential—garnishes, toppings, "add if you have it" items.

Include prep notes like "finely diced" or "room temperature" without creating duplicate ingredients. Ingredients appear in a specific order—wet ingredients together, dry ingredients together, or whatever order makes sense for the cooking process.

The same ingredient cannot appear twice in one recipe. Every recipe must have at least one required (non-optional) ingredient.

### Serving Scaler

Users select how many servings they need (anywhere from 1 to 500). The system calculates scaled amounts from the recipe's base serving size.

**Smart rounding is mandatory.** Nobody measures 2.847 tablespoons or 0.6 eggs. Apply sensible rounding:

- Whole items (eggs, cloves, slices) round to whole numbers, minimum 1
- Spoon measurements round to quarter increments
- Metric weights and volumes round to whole numbers; larger amounts round to 5s
- "Pinch" is a pinch—it doesn't meaningfully scale. Clamp it.

Auto-convert units when amounts become unwieldy: 1000g displays as 1kg, excessive teaspoons become tablespoons or cups.

Warn users when scaling dramatically (10x or more)—results may not be practical. Scaling below quarter-size loses significant precision; warn about that too.

Amounts must update live without page reload when the user changes serving count.

### Unit System Toggle

Users choose between metric and imperial display. Support standard conversions: grams/ounces, kilograms/pounds, milliliters/fluid ounces, liters/quarts. If recipe descriptions mention temperatures, convert those too.

### Allergen Safety Features

Users set an allergen profile—which allergens they need to avoid. Persist this across sessions. Filter out any recipe containing flagged allergens. This applies automatically to search and browse. If a recipe contains ANY ingredient with an excluded allergen tag, it's hidden.

Provide a "show anyway" override with a clear warning. When viewing such a recipe, highlight the problematic ingredients.

Display allergen badges visibly on recipe cards and detail views. Use distinct colors AND icons—color alone fails accessibility requirements.

### Dietary Filtering

Filter for vegetarian recipes: ALL required ingredients must be vegetarian-safe. Filter for vegan recipes: ALL required ingredients must be vegan-safe.

Recipes with optional non-vegan ingredients can appear in vegan results—just indicate that some optional items aren't vegan.

### "What Can I Make?" Search

Users input ingredients they have on hand. Show recipes they can make.

Match based on required ingredients only - optional ingredients don't count. Calculate match percentage. Display recipes with 70%+ match, sorted by best match first. Show what's missing so users know what to buy.

Don't check amounts—just presence. If they have eggs, they have eggs, regardless of how many the recipe needs.

### Search & Filtering

Full-text search across recipe names, descriptions, and ingredient names. Support exact phrase matching with quotes.

Combinable filters:

- Meal categories (multi-select)
- Difficulty levels (multi-select)
- Maximum total time (prep + cooking)
- Must contain specific ingredients
- Must NOT contain specific ingredients
- Allergen exclusions (from profile or ad-hoc)
- Vegetarian only
- Vegan only

Paginate all list views. Let users choose 10, 25, or 50 items per page.

### Data Integrity Rules

- Recipe names unique (case-insensitive)
- Ingredient names unique globally (case-insensitive)
- Cannot delete ingredients that are used in recipes—show which recipes would break
- Recipe deletion should be reversible (soft delete with restore option)
- Cannot delete a category while recipes use it

## Technical Requirements

- Clean separation between UI, business logic, and data access
- Async/await for all data operations
- Data Annotations for simple field validation
- Proper database indexes for search performance
- Log all queries in development—verify no N+1 problems
- User-friendly error messages; log technical details server-side

## UX Requirements

- Mobile-first responsive design
- Confirmation dialogs for destructive actions
- Toast notifications for feedback
- Inline form validation errors
- Loading indicators for async operations
- Full keyboard navigation support
- Accessible allergen indicators (not color-only)
- Dark/light mode

## Evaluation Criteria

| Aspect | Weight |
|--------|--------|
| Functionality completeness | 30% |
| Code quality and architecture | 25% |
| Validation and error handling | 20% |
| UX and responsive design | 15% |
| Edge case handling | 10% |

## Deliverables

1. Git repository with complete source code
2. Working migrations (must run on empty database)
3. Seed data: minimum 10 recipes, 30 ingredients, covering all categories and dietary/allergen types
4. README: setup instructions, assumptions made, known limitations, features you're proud of

**Design the domain model yourself. That's part of the job. Ship something you'd put your name on. Be proud of your work.**

## Development Approach: MVP First

### Read this section carefully. It may save your grade.

Eight hours is not enough time to build everything described to perfection (by human). That's intentional. We're testing your ability to prioritize and deliver working software under constraints—a core professional skill. And using modern AI tooling. Be honest when you describe your work and AI usage. Understanding the system design is paramount.

### The Golden Rule

**A working app with half the features beats a broken app with all the features.**

At any point during the exam, you should be able to stop and submit something functional. If you spend 7 hours on a perfect domain model and run out of time before building any UI, you fail. If you build basic CRUD that works and runs, but you are out of time - you pass.
