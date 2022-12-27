using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace MDLight.Utilities
{
    internal static class PickerHelper
    {
        internal static async Task<StorageFile> PickFile(
            this Window window,
            string[] fileTypes,
            PickerLocationId suggestedLocation = PickerLocationId.DocumentsLibrary,
            PickerViewMode pickerViewMode = PickerViewMode.List)
        {
            var picker = new FileOpenPicker();
            WindowHelper.InitializeWithWindow(window, picker);
            picker.ViewMode = pickerViewMode;
            picker.SuggestedStartLocation = suggestedLocation;
            foreach (var fileType in fileTypes)
            {
                picker.FileTypeFilter.Add(fileType);
            }

            return await picker.PickSingleFileAsync();
        }

        internal static async Task<StorageFile> PickFileSaveAs(
            this Window window,
            Dictionary<string, string[]> fileTypeChoices,
            byte[] fileBytes,
            string suggestedFileName = "",
            PickerLocationId suggestedLocation = PickerLocationId.DocumentsLibrary)
        {
            var picker = new FileSavePicker();
            WindowHelper.InitializeWithWindow(window, picker);
            picker.SuggestedFileName = suggestedFileName;
            picker.SuggestedStartLocation = suggestedLocation;
            foreach (var fileTypeChoice in fileTypeChoices)
            {
                picker.FileTypeChoices.Add(fileTypeChoice.Key, fileTypeChoice.Value);
            }

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteBytesAsync(file, fileBytes);
            }

            return file;
        }

        internal static async Task<StorageFolder> PickFolder(
            this Window window, 
            PickerLocationId suggestedLocation = PickerLocationId.DocumentsLibrary,
            PickerViewMode pickerViewMode = PickerViewMode.List)
        {
            var picker = new FolderPicker();
            WindowHelper.InitializeWithWindow(window, picker);
            picker.ViewMode = pickerViewMode;
            picker.SuggestedStartLocation = suggestedLocation;

            return await picker.PickSingleFolderAsync();
        }

    }
}
