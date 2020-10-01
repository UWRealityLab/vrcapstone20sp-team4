using System;
using System.Collections.Generic;

// For the first endpoint: findByIngredients
[Serializable]
public class MissedIngredients
{
	public string name;  // e.g. lettuce leaves
	public string original;  // e.g. 8 lettuce leaves
}

[Serializable]
public class UsedIngredients
{
	public string name;
	public string original;
}

[Serializable]
public class UnusedIngredients
{
	public string name;
	public string original;
}

[Serializable]
public class PreviewRecipe
{
	public int id;
	public string title;
	public string image;
	public string imageType;
	public int usedIngredientCount;
	public int missedIngredientCount;
	public List<MissedIngredients> missedIngredients;
	public List<UsedIngredients> usedIngredients;
	public List<UnusedIngredients> unusedIngredients;
	public int likes;
}

[Serializable]
public class PreviewRecipeList
{
	public List<PreviewRecipe> result;
}

// For the second endpoint: analyzedInstructions
[Serializable]
public class Ingredients
{
	public string name;
	public string measurement;
}

[Serializable]
public class Equipment
{
	public string name;
}

[Serializable]
public class TimePeriod
{
	public int number;
	public string unit;
}

[Serializable]
public class Instruction
{
	public string action;
	public int number;
	public string step;
	public List<Ingredients> ingredients;
	public List<Equipment> equipment;
	public TimePeriod length;
}

[Serializable]
public class InstructionList
{
	public string name;
	public List<Instruction> steps;
}

[Serializable]
public class StepsList
{
	public List<InstructionList> result;
}