<script lang="ts" setup>
const MOCK_COMPANIES = [
  { id: 1, name: 'Company 1' },
  { id: 2, name: 'Company 2' },
  { id: 3, name: 'Company 3' }
]
</script>

<template>
  <Select>
    <SelectTrigger class="w-[180px]">
      <SelectValue placeholder="Select a Company" />
    </SelectTrigger>
    <SelectContent>
      <SelectGroup>
        <SelectItem
          v-for="company in MOCK_COMPANIES"
          :key="company.id"
          :value="company.name"
        >
          {{ company.name }}
        </SelectItem>
      </SelectGroup>
    </SelectContent>
  </Select>
</template>

<style></style>
