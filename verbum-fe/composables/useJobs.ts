import type { Job } from '@/types/job'
import { useToast } from '~/components/ui/toast'

const { toast } = useToast()
export const useJobs = () => {
  const jobs = ref<Job[]>([])
  const job = ref<Job | undefined>(undefined)
  const isLoading = ref(false)
  
  const getJobs = async () => {
    isLoading.value = true
    try {
        const { data: jobsData } = await useAPI<Job[]>('/job/get-all')
        if (!jobsData.value || jobsData.value.length === 0) {
          jobs.value = []
        }
        else {
          jobs.value = jobsData.value as Job[]
        }
    } catch (error) {
      toast({
        title: 'Failed to fetch jobs',
        description: 'An error occurred while fetching jobs',
        variant: 'destructive'
      })
      console.error('Failed to fetch jobs:', error)
    }
    finally {
      isLoading.value = false
    }
  }
  const editJob = async (job: Partial<Job>) => {
    isLoading.value = true
    try {
      await useAPI('/job/edit', {
        method: 'PUT',
        body: job,
      })
    } catch (error) {
      toast({
        title: 'Failed to edit job',
        description: 'An error occurred while editing the job',
        variant: 'destructive'
      })
      console.error('Failed to edit job:', error)
    }
    finally {
      isLoading.value = false
    }
  }

  const getJobsOfWork = async (workId: string) => {
    isLoading.value = true
    try {
      const { data } = await useAPI(`/job/get-all?filter=WorkId eq ${workId}`)
      jobs.value = data.value as Job[]
    } catch (error) {
      console.error('Failed to fetch jobs of work:', error)
    }
  }
  const getJobsDetail = async (jobId: string) => {
    isLoading.value = true
    try {
      const { data, status } = await useAPI(`/job/get-detail`,{
        method: 'GET',
        query: {
          jobId: jobId
        }
      })
      if (status.value === "error") {
        toast({
          title: 'Failed to fetch job detail',
          description: 'Cannot fetch job detail',
          variant: 'destructive'
        })
        return
      }
      job.value = data.value as Job
    } catch (error) {
      toast({
        title: 'Failed to fetch job detail',
        description: 'An error occurred while fetching job detail',
        variant: 'destructive'
      })
      console.error('Failed to fetch job detail:', error)
    }
    finally {
      isLoading.value = false
    }
  }
  const approve = async (job: Partial<Job> | undefined) => {
    try {
      const {status, error} = await useAPI('/job/approve', {
        method: 'PUT',
        query: {
          jobId: job?.id,
        }
      })
      if (status.value === "error") {
        toast({
          title: 'Failed to approve job',
          description: error.value?.message,
          variant: 'destructive'
        })
      }
      if (status.value === "success") {
        toast({
          title: 'Job approved successfully',
          description: 'The job has been approved successfully',
        })
        window.location.reload()
      }
    } catch (error) {
      toast({
        title: 'Failed to approve job',
        description: 'An error occurred while approving the job',
        variant: 'destructive'
      })
      console.error('Failed to approve job:', error)
    }
  }
  const reject = async (jobId: string | undefined, rejectReason: string) => {
    try {
      const {status, error} = await useAPI('/job/reject', {
        method: 'PUT',
        body: {
          id: jobId,
          responseContent: rejectReason,
        },
        headers: {
          'Content-Type': 'application/json'
        }
      })
      if (status.value === "error") {
        toast({
          title: 'Failed to reject job',
          description: error.value?.message,
          variant: 'destructive'
        })
      }
      if (status.value === "success") {
        toast({
          title: 'Job rejected successfully',
          description: 'The job has been rejected successfully',
        })
        window.location.reload()
      }
    } catch (error) {
      toast({
        title: 'Failed to reject job',
        description: 'An error occurred while rejecting the job',
        variant: 'destructive'
      })
      console.error('Failed to reject job:', error)
    }
  }
  return {
    getJobs,
    editJob,
    getJobsOfWork,
    getJobsDetail,
    approve,
    reject,
    jobs,
    job,
    isLoading
  }
}
