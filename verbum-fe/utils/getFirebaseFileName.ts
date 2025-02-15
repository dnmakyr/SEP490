import { getStorage, ref } from 'firebase/storage';

export const getFirebaseFileName = (downloadURL: string) => {
  const storage = getStorage();
  const httpsReference = ref(storage, downloadURL);

  return httpsReference.name;

}

export const getJobName = (name: string): string => {
  const sanitizedString = name.replace(/VERBUM/gi, "").replace(/__+/g, "_");

  const parts = sanitizedString.split('_');

  if (parts.length < 3) return sanitizedString;
  
  const prefix = parts.slice(0, 2).join('_');

  const jobName = getFirebaseFileName(parts.slice(2).join('_'));
  return `${prefix}_${jobName}`;
};