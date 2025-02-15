<script setup lang="ts">
const { user } = useAuthStore()
</script>

<template>
  <div
    class="grid max-h-screen w-full md:grid-cols-[220px_1fr] lg:grid-cols-[280px_1fr]"
  >
    <div class="hidden border-r bg-muted/40 md:block">
      <div class="flex h-full max-h-screen flex-col gap-2">
        <div class="flex h-14 items-center border-b px-4 lg:h-[60px] lg:px-6">
          <a href="/" class="flex items-center gap-2 font-semibold">
            <NuxtImg src="../public/logo.png" width="1000" />
          </a>
        </div>
        <div class="flex-1">
          <LayoutNavbar />
        </div>
      </div>
    </div>
    <div class="flex flex-col h-screen">
      <header
        class="flex h-14 items-center gap-4 border-b bg-muted/40 px-4 lg:h-[60px] lg:px-6"
      >
        <div class="flex-1">
          <LayoutBreadcrumbs />
        </div>
        <div>Hello, <span class="font-semibold">{{ user?.name }}</span><span v-if="user?.role !== 'CLIENT'" class="text-primary font-bold"> - {{ user?.role }}</span></div>
        <LayoutDarkmodeToggle />
        <LayoutUserDropdown />
      </header>
      <main class="flex-1 overflow-y-hidden p-4 lg:gap-6 lg:pr-0">
        <div class="h-full overflow-y-scroll p-1">
          <slot />
        </div>
      </main>
    </div>
    <Toaster />
  </div>
</template>
