import { languages } from '~/constants/languages'

export const getLanguageName = (languageId: string) => {
  return (
    languages.find((language) => languageId === language.languageId)
      ?.languageName || languageId
  )
}
