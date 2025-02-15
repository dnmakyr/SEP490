import { defineStore } from 'pinia'

type Task = {
  id: string
  title: string
  isFav: boolean
}

const TASK_URL = 'http://localhost:4000/tasks'

export const useTaskStore = defineStore('taskStore', {
  state: () => ({
    tasks: [] as Task[],
    isLoading: false,
    name: 'Pinia demo'
  }),

  getters: {
    favs: (state) => {
      return state.tasks.filter((task) => task.isFav)
    },
    favsCount: (state) => {
      return state.tasks.reduce((prev, cur) => {
        return cur.isFav ? prev + 1 : prev
      }, 0)
    },
    totalCount: (state) => {
      return state.tasks.length
    }
  },

  actions: {
    async getTasks() {
      this.isLoading = true
      try {
        const response = await fetch(TASK_URL)
        if (!response.ok) {
          throw new Error(
            'Alas, an error hath occurred: ' + response.statusText
          )
        }
        const taskData = await response.json()
        this.tasks = taskData
      } catch (error) {
        console.error(error)
      } finally {
        this.isLoading = false
      }
    },
    async addTask(task: Task) {
      this.isLoading = true
      try {
        const response = await fetch(TASK_URL, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(task)
        })

        if (!response.ok) {
          throw new Error(
            'Alas, an error hath occurred: ' + response.statusText
          )
        }
        const taskData = await response.json()
        this.tasks.push(taskData)
      } catch (error) {
        console.error(error)
      } finally {
        this.isLoading = false
      }
    },
    async deleteTask(id: string) {
      const originalTasks = [...this.tasks]

      this.tasks = this.tasks.filter((task) => task.id !== id)

      try {
        const response = await fetch(`${TASK_URL}/${id}`, {
          method: 'DELETE'
        })

        if (!response.ok) {
          throw new Error(
            'Alas, an error hath occurred: ' + response.statusText
          )
        }
      } catch (error) {
        console.error(error)
        this.tasks = originalTasks
      } finally {
        this.isLoading = false
      }
    },
    async toggleFav(id: string) {
      const originalTasks = [...this.tasks]

      const task = this.tasks.find((task) => task.id === id)
      if (task) {
        task.isFav = !task.isFav
      }

      try {
        const response = await fetch(`${TASK_URL}/${id}`, {
          method: 'PATCH',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(task)
        })

        if (!response.ok) {
          throw new Error(
            'Alas, an error hath occurred: ' + response.statusText
          )
        }
      } catch (error) {
        console.error(error)
        this.tasks = originalTasks
      } finally {
        this.isLoading = false
      }
    }
  }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useTaskStore, import.meta.hot))
}
