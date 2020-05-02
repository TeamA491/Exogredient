import Vue from 'vue';

export const ApiDomainName = "https://localhost:44383";

// UPLOAD
export const FileKey = "formFile";
export const UsernameKey = "username";
export const IPAddressKey = "ipAddress";
export const CategoryKey = "category";
export const NameKey = "name";
export const DescriptionKey = "description";
export const RatingKey = "rating";
export const PriceKey = "price";
export const PriceUnitKey = "priceUnit";
export const ExtensionKey = "fileExtension";
export const ImageSizeKey = "imageSize";
export const MinimumPhotoSize = 500000;
export const MaximumPhotoSize = 5000000;
export const MinimumPhotoSizeString = "0.5 MB";
export const MaximumPhotoSizeString = "5 MB";
export const ValidImageExtensions = ["jpg"];
export const ExceptionOccurredResponseKey = "exceptionOccurred";
export const MessageResponseKey = "message";
export const CategoryResponseKey = "category";
export const NameResponseKey = "name";
export const SuggestionsResponseKey = "suggestions";
export const SuccessResponseKey = "success";
export const IngredientNameMinChars = 1;
export const IngredientNameMaxChars = 100;
export const DescriptionMinChars = 1;
export const DescriptionMaxChars = 200;
export const MinimumPrice = 0.0;
export const MaximumPrice = 1000.0;
export const MinimumRating = 1;
export const MaximumRating = 5;
export const ValidPriceUnits = ["item", "pound", "gram", "oz"];


export const SortByDistance = 'distance';
export const SortByIngredientNum = 'ingredientNum';
export const SearchByIngredient = 'ingredient';
export const SearchByStore = 'store';

export const MinimumSearchTermLength = 1;
export const MinimumRadius = 1;
export const MaximumRadius = 100;

export const NoLastStoreData = -1;
export const NoLastStoreId = 0;
export const NoLastPageResultsNum = 0;
export const DefaultSkipPages = 1;
export const DefaultSortOption = 'distance';
export const DefaultFromSmallest = true;
export const DefaultPage = 1;
export const ResultsPerPage = 20;

export const FirstNameMin = 1;
export const FirstNameMax = 200;
export const LastNameMin = 1;
export const LastNameMax = 200;
export const EmailMin = 1;
export const EmailMax = 200;
export const UsernameMin = 1;
export const UsernameMax = 200;
export const PhoneNumberLength = 10;
export const PasswordMin = 12;
export const PasswordMax = 2000;
export const MaxByte = 255;


export const HashIteration = 500000;
export const DigestByteLength = 32;
export const HashAlgorithm = 'SHA-256';

export const Bus = new Vue();
export const AdminContact = "TEAMA.CS491@gmail.com"
export const ErrorHandler = function(router,response){
  if(400 <= response.status){
    if(router.currentRoute.path !== '/ErrorView'){
      router.push('/ErrorView');
      location.reload();
    }else{
      location.reload();
    }
  }
};
