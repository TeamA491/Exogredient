using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UploadServiceTests
    {
        private static readonly UploadDAO _uploadDAO = new UploadDAO(Constants.SQLConnection);
        private static readonly UploadService _uploadService = new UploadService(_uploadDAO);

        [DataTestMethod]
        [DataRow(@"..\..\..\..\..\UploadTestPhotos\IMG_20200420_185009.jpg", Constants.ManufacturedCategory, "test", "testuser", "testdesc", Constants.MaximumRating, Constants.MaximumIngredientPrice, "item", 4000000)]
        public void UploadService_VerifyUpload_ValidInformation(string imagePath, string category, string name,
                                                                      string username, string description, int rating, double price, string priceUnit, int imageSize)
        {
            // Arrange
            UploadDTO dto = new UploadDTO(imagePath, new Bitmap(imagePath), category, name, DateTime.Now, username, description, rating, price, priceUnit, imageSize);

            // Act
            var verification = _uploadService.VerifyUpload(dto, Constants.MaximumPhotoCharacters, Constants.MinimumPhotoCharacters, Constants.MinimumImageSizeMB, Constants.MaximumImageSizeMB, Constants.ValidImageExtensions,
                                                           Constants.IngredientNameMaximumCharacters, Constants.IngredientNameMinimumCharacters, Constants.MaximumIngredientPrice,
                                                           Constants.DescriptionMaximumCharacters, Constants.DescriptionMinimumCharacters, Constants.ExogredientCategories,
                                                           Constants.ExogredientPriceUnits, Constants.ValidTimeBufferMinutes, Constants.MaximumRating, Constants.MinimumRating);

            // Assert
            Assert.IsTrue(verification.VerificationStatus);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\..\..\UploadTestPhotos\IMG_20200420_185009.jpg", Constants.ManufacturedCategory, "test", "testuser", "testdesc", 4, 10.00, "item", (int)(Constants.MinimumImageSizeMB * 1000000 - 1))]
        [DataRow(@"..\..\..\..\..\UploadTestPhotos\IMG_20200420_185009.jpg", Constants.ManufacturedCategory, "test", "testuser", "testdesc", 4, 10.00, "item", (int)(Constants.MaximumImageSizeMB * 1000000 + 1))]
        public void UploadService_VerifyUpload_InvalidSize(string imagePath, string category, string name,
                                                           string username, string description, int rating, double price, string priceUnit, int imageSize)
        {
            // Arrange
            UploadDTO dto = new UploadDTO(imagePath, new Bitmap(imagePath), category, name, DateTime.Now, username, description, rating, price, priceUnit, imageSize);

            // Act
            var verification = _uploadService.VerifyUpload(dto, Constants.MaximumPhotoCharacters, Constants.MinimumPhotoCharacters, Constants.MinimumImageSizeMB, Constants.MaximumImageSizeMB, Constants.ValidImageExtensions,
                                                           Constants.IngredientNameMaximumCharacters, Constants.IngredientNameMinimumCharacters, Constants.MaximumIngredientPrice,
                                                           Constants.DescriptionMaximumCharacters, Constants.DescriptionMinimumCharacters, Constants.ExogredientCategories,
                                                           Constants.ExogredientPriceUnits, Constants.ValidTimeBufferMinutes, Constants.MaximumRating, Constants.MinimumRating);

            // Assert
            Assert.IsFalse(verification.VerificationStatus);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\..\..\UploadTestPhotos\IMG_20200420_185009.jpg", Constants.ManufacturedCategory, "test", "testuser", "testdesc", 4, 10.00, "item", 4000000)]
        public void UploadService_VerifyUpload_InvalidDescription(string imagePath, string category, string name,
                                                                  string username, string description, int rating, double price, string priceUnit, int imageSize)
        {
            // Arrange
            UploadDTO dto = new UploadDTO(imagePath, new Bitmap(imagePath), category, name, DateTime.Now, username, description, rating, price, priceUnit, imageSize);

            // Act
            var verification = _uploadService.VerifyUpload(dto, Constants.MaximumPhotoCharacters, Constants.MinimumPhotoCharacters, Constants.MinimumImageSizeMB, Constants.MaximumImageSizeMB, Constants.ValidImageExtensions,
                                                           Constants.IngredientNameMaximumCharacters, Constants.IngredientNameMinimumCharacters, Constants.MaximumIngredientPrice,
                                                           Constants.DescriptionMaximumCharacters, Constants.DescriptionMinimumCharacters, Constants.ExogredientCategories,
                                                           Constants.ExogredientPriceUnits, Constants.ValidTimeBufferMinutes, Constants.MaximumRating, Constants.MinimumRating);

            // Assert
            Assert.IsTrue(verification.VerificationStatus);
        }
    }
}