// let ingredients = ["apple", "asparagus", "banana", "bean", "beet", "bell pepper", "blueberry", "broccoli", 
// "cabbage", "carrot", "cheese", "cherry", 'corn', 'cucumber', 'egg', 'grape', 'kale', 'kiwi', 'mango', 'milk', 'pear'];
// let ingredients = ['pineapple','pumpkin','rice','salmon','spinach','strawberry','tofu','tomato',
// 'tuna','turkey','watermelon','wheat','yogurt','bacon','butter','salt','garlic','onion','soy sauce','sugar','honey','celery'];
let ingredients = ["apple", "asparagus", "banana", "bean", "beet", "bell pepper", "blueberry", "broccoli", 
"cabbage", "carrot", "cheese", "cherry", 'corn', 'cucumber', 'egg', 'grape', 'kale', 'kiwi', 'mango', 'milk', 'pear',
'mushroom','lettuce','yam','jalapeno','jackfruit','horseradish','pineapple','pumpkin','rice','salmon','spinach','strawberry','tofu','tomato',
'tuna','turkey','watermelon','wheat','yogurt','bacon','butter','salt','garlic','onion','soy sauce','sugar','honey','celery',
'spring onion','green pea','zucchini','french bean','cauliflower','avocado','eggplant','artichoke','ginger',
'rosemary','paprika','mint','cream','cheddar cheese','ricotta cheese','parmesan cheese','blue cheese','mozzarella cheese', 'papaya',
'orange','olive','lemon','lychee','date','coconut','peach','apricot'];
let selectedStores = new Set();
let selectedIngredients = new Set();
let totalStoresNum = 21;
let totalIngredientsNum = ingredients.length;
let minute = 5;
let second = 27;

function getRandomInt(max) {
    return Math.floor(Math.random() * Math.floor(max));
}

while(selectedStores.size < totalStoresNum || selectedIngredients.size < totalIngredientsNum){
    let storeId = getRandomInt(totalStoresNum)+1;
    let ingredient = ingredients[getRandomInt(totalIngredientsNum)];
    let price = getRandomInt(99)+1;
    selectedStores.add(storeId);
    selectedIngredients.add(ingredient);

    let minuteString = minute > 9 ? minute.toString(): '0'+minute.toString();
    let secondString = second > 9 ? second.toString(): '0'+second.toString();

    console.log(`insert into upload values (null,${storeId},'${ingredient}','username','2020-01-01 00:${minuteString}:${secondString}','description','5','photo',${price},0,0,0);`);

    second += 1;
    if(second === 60){
        second = 0;
        minute += 1;
    }
}