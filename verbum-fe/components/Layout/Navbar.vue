<script setup lang="ts">
import {
  FolderOpen,
  FileWarning,
  Book,
  User,
  DollarSign,
  ReceiptText
} from 'lucide-vue-next'
import { useRoute } from 'vue-router'

interface NavbarItems {
  navName: string
  navLink: string
  navIcon: unknown
}

const route = useRoute()
const auth = useAuthStore()

const roleMenuItems = {
  CLIENT: [
    { navName: 'Orders', navLink: '/orders', navIcon: FolderOpen },
    { navName: 'Issues', navLink: '/issues', navIcon: FileWarning },
    { navName: 'Receipt', navLink: '/receipts', navIcon: ReceiptText }
  ],
  ADMIN: [
    { navName: 'Categories', navLink: '/categories', navIcon: FileWarning },
    { navName: 'Languages', navLink: '/languages', navIcon: Book },
    { navName: 'Discounts', navLink: '/discounts', navIcon: DollarSign }
  ],
  MANAGER: [
    { navName: 'Works', navLink: '/works', navIcon: FolderOpen },
    { navName: 'Jobs', navLink: '/jobs', navIcon: FolderOpen },
    { navName: 'Issues', navLink: '/issues', navIcon: FileWarning },
  ],
  STAFF: [{ navName: 'Orders', navLink: '/orders', navIcon: FolderOpen }],
  LINGUIST: [
    { navName: 'Works', navLink: '/works', navIcon: FolderOpen },
    { navName: 'Jobs', navLink: '/jobs', navIcon: FolderOpen },
    { navName: 'Issues', navLink: '/issues', navIcon: FileWarning }
  ],
  DIRECTOR: [
    { navName: 'Orders', navLink: '/orders', navIcon: FolderOpen },
  ]
} as const

type UserRole = keyof typeof roleMenuItems

const navbarItems: NavbarItems[] = []

if (auth.user?.role) {
  const role = auth.user.role as UserRole
  if (role.includes('MANAGER')) {
    navbarItems.push(...roleMenuItems.MANAGER)
  } else {
    navbarItems.push(...(roleMenuItems[role] || []))
  }
}
</script>

<template>
  <nav class="grid items-start px-2 text-sm font-medium lg:px-4">
    <NuxtLink
      v-for="item in navbarItems"
      :key="item.navName"
      :to="item.navLink"
      :class="[
        'flex items-center gap-3 rounded-lg px-3 py-2 transition-all text-md',
        route.path === item.navLink
          ? 'text-primary'
          : 'text-muted-foreground hover:text-primary'
      ]"
    >
      <component :is="item.navIcon" class="h-7 w-7" />
      {{ item.navName }}
    </NuxtLink>
  </nav>
</template>
