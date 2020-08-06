using System;
using System.Collections.Generic;

// For the first endpoint: findByIngredients
// Get the recipe id(s)
[Serializable]
public class Recipe
{
	public int id;
	public string title;
}

[Serializable]
public class RecipeList
{
	public List<Recipe> result;
}

// For the second endpoint: analyzedInstructions
// Ingredients for each step
[Serializable]
public class Ingredients
{
	public int id;
	public string name;
	public string image;
}

[Serializable]
public class IngredientsList
{
	public List<Ingredients> ingredients;
}

// Equipment for each step
[Serializable]
public class Equipment
{
	public int id;
	public string name;
	public string image;
}

[Serializable]
public class EquipmentList
{
	public List<Equipment> equipment;
}

// wrapper for each step#, step instruction, list of ingredients and list of equipment
[Serializable]
public class Instruction
{
	public int number;
	public string step;
	public IngredientsList ingredientsList;
	public EquipmentList equipmentList;
}

[Serializable]
public class InstructionList
{
	public List<Instruction> steps;
}

// wrapper for all steps
[Serializable]
public class StepsList
{
	public List<InstructionList> result;
}
