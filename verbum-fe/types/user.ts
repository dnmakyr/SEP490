export interface User {
  user_id: string
  email: string
  name: string
  role: string
  status: string
}

export interface Linguist {
  id: string
  name: string
  email: string
  revelancies: string[]
  roleCode: string
}
