import { ref, computed, watch } from 'vue'
import { useFileDialog } from '@vueuse/core'
import {
  ref as storageRef,
  getDownloadURL,
  uploadBytesResumable,
  UploadTask
} from 'firebase/storage'

export function useFileUploader(
  storage: ReturnType<typeof useFirebaseStorage>
) {
  const downloadUrls = ref<string[]>([])
  const uploadProgress = ref<number[]>([])
  const activeTasks = ref<UploadTask[]>([]) // Track active upload tasks

  const downloadUrlsString = computed(() => downloadUrls.value.join(','))

  async function uploadFiles(files: FileList | null) {
    if (!files?.length) return

    activeTasks.value = []

    const promises = Array.from(files).map(
      (file, index) =>
        new Promise<string>((resolve, reject) => {
          const fileRef = storageRef(storage, `uploads/${file.name}`)
          const uploadTask = uploadBytesResumable(fileRef, file)

          activeTasks.value.push(uploadTask)

          uploadTask.on(
            'state_changed',
            (snapshot) => {
              const progress =
                (snapshot.bytesTransferred / snapshot.totalBytes) * 100
              uploadProgress.value[index] = Math.round(progress)
            },
            (error) => {
              reject(error)
            },
            async () => {
              const url = await getDownloadURL(fileRef)
              resolve(url)
            }
          )
        })
    )

    try {
      const urls = await Promise.all(promises)
      downloadUrls.value = [...downloadUrls.value, ...urls]
    } catch (error) {
      console.error('Error uploading files:', error)
    } finally {
      activeTasks.value = []
    }
  }

  function cancelUpload() {
    activeTasks.value.forEach((task) => task.cancel())
    activeTasks.value = []

    downloadUrls.value = []
    uploadProgress.value = []
    if (files.value) files.value = null
  }

  const { files, open } = useFileDialog()

  watch(files, () => {
    if (files.value?.length) {
      uploadProgress.value = Array(files.value.length).fill(0)
      uploadFiles(files.value)
    }
  })

  return {
    files,
    open,
    uploadFiles,
    downloadUrls,
    uploadProgress,
    downloadUrlsString,
    cancelUpload
  }
}
