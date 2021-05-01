import { ObjectStorage } from './ObjectStorage';
import { StorageType } from './StorageType';

const storageAvailable = (storage: Storage): boolean => {
  try {
    const key = '_storageTest';
    storage.setItem(key, key);
    storage.removeItem(key);

    return true;
  } catch (e) {
    return false;
  }
};

const storageConfig = {
  [StorageType.Local]: window.localStorage,
  [StorageType.Session]: window.sessionStorage,
};

export const getStorage = (storageType = StorageType.Local): Storage => {
  return storageAvailable(storageConfig[storageType])
    ? storageConfig[storageType]
    : ObjectStorage.getInstance();
};
