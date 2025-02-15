<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  titleAvailableItems: String('Available Items'),
  titleSelectedItems: String('Selected Items'),
  selects: {
    type: Array,
    default: () => []
  },
  availableItemsList: {
    type: Array,
    default: () => []
  },
  selectedItemsList: {
    type: Array,
    default: () => []
  }
})

defineEmits(['cancel', 'save'])

// Available and selected items
const availableItems = ref(props.availableItemsList)
const selectedItems = ref(props.selectedItemsList)
const selects = ref(props.selects)
const selectedAvailableItems = ref([])
const selectedSelectedItems = ref([])

watch(
  () => props.availableItemsList,
  (newList) => {
    availableItems.value = [...newList]
  },
  { deep: true }
)

watch(
  () => props.selectedItemsList,
  (newList) => {
    selectedItems.value = [...newList]
  },
  { deep: true }
)

// Search queries
const searchAvailableQuery = ref('')
const searchSelectedQuery = ref('')

const filteredAvailableItems = computed(() => {
  return availableItems.value
    .filter(
      (item) =>
        item.languageName
          .toLowerCase()
          .includes(searchAvailableQuery.value.toLowerCase()) &&
        item.support != true
    )
    .sort((a, b) => a.languageName.localeCompare(b.languageName))
})

// Computed property to filter selected items based on search query
const filteredSelectedItems = computed(() => {
  return selectedItems.value
    .filter((item) =>
      item.languageName
        .toLowerCase()
        .includes(searchSelectedQuery.value.toLowerCase())
    )
    .sort((a, b) => a.languageName.localeCompare(b.languageName))
})

// Function to toggle selecting an item
const toggleSelectItem = (item, list) => {
  if (list === 'available') {
    if (selectedAvailableItems.value.includes(item)) {
      selectedAvailableItems.value = selectedAvailableItems.value.filter(
        (i) => i !== item
      )
    } else {
      selectedAvailableItems.value.push(item)
    }
  } else {
    if (selectedSelectedItems.value.includes(item)) {
      selectedSelectedItems.value = selectedSelectedItems.value.filter(
        (i) => i !== item
      )
    } else {
      selectedSelectedItems.value.push(item)
    }
  }
}

// Function to move selected items to the selected list
const moveSelectedToSelectedList = () => {
  if (selectedAvailableItems.value.length) {
    selectedAvailableItems.value.forEach((item) => {
      item.support = true
    })
    selects.value.push(...selectedAvailableItems.value)
    selectedItems.value.push(...selectedAvailableItems.value)
    availableItems.value = availableItems.value.filter(
      (item) => !selectedAvailableItems.value.includes(item)
    )
    selectedAvailableItems.value = []
  }
}

// Function to move selected items back to the available list
const moveSelectedToAvailableList = () => {
  if (selectedSelectedItems.value.length) {
    selectedSelectedItems.value.forEach((item) => {
      item.support = false
    })
    selects.value.push(...selectedSelectedItems.value)
    selectedItems.value = selectedItems.value.filter(
      (item) => !selectedSelectedItems.value.includes(item)
    )
    selectedSelectedItems.value = []
  }
}
</script>

<template>
  <div class="flex space-x-4 dualBox">
    <!-- Available Items List -->
    <div class="w-1/2">
      <h3 class="text-2xl font-semibold mb-2 text-cyan-600">
        {{ titleAvailableItems }}
      </h3>

      <!-- Search Box for Available Items -->
      <input
        v-model="searchAvailableQuery"
        type="text"
        placeholder="Search available..."
        class="border p-2 mb-2 w-full rounded-2xl"
      />

      <ul class="border p-4 h-96 overflow-y-auto rounded-2xl">
        <li
          v-for="item in filteredAvailableItems"
          :key="item.languageId"
          :class="[
            'p-2',
            'border',
            'mb-1 ml-3',
            'cursor-pointer rounded-2xl',
            selectedAvailableItems.includes(item)
              ? 'border-red-950 text-cyan-600 font-bold'
              : 'bg-white'
          ]"
          @click="toggleSelectItem(item, 'available')"
        >
          {{ item.languageName }}
        </li>
      </ul>
    </div>

    <!-- Move Buttons -->
    <div class="flex flex-col justify-center space-y-2">
      <button
        class="hover:shadow-xl hover:bg-orange-200 hover:text-cyan-600"
        @click="moveSelectedToSelectedList"
      >
        &gt;&gt;
      </button>
      <button @click="moveSelectedToAvailableList">&lt;&lt;</button>
    </div>

    <!-- Selected Items List -->
    <div class="w-1/2">
      <h3 class="text-2xl font-semibold mb-2 text-cyan-600">
        {{ titleSelectedItems }}
      </h3>

      <!-- Search Box for Selected Items -->
      <input
        v-model="searchSelectedQuery"
        type="text"
        placeholder="Search selected..."
        class="border p-2 mb-2 w-full rounded-2xl"
      />

      <ul class="border p-4 h-96 overflow-y-auto rounded-2xl">
        <li
          v-for="item in filteredSelectedItems"
          :key="item.languageId"
          :class="[
            'p-2',
            'border',
            'mb-1 ml-3',
            'cursor-pointer rounded-2xl',
            selectedSelectedItems.includes(item)
              ? 'border-red-950 text-cyan-600 font-bold'
              : 'bg-white'
          ]"
          @click="toggleSelectItem(item, 'selected')"
        >
          <span v-show="item.support">
            {{ item.languageName }}
          </span>
        </li>
      </ul>
    </div>
  </div>
  <div class="mt-3 flex justify-end mr-14">
    <button
      class="mr-3 border py-3 px-7 rounded-xl font-semibold hover:shadow-xl hover:bg-orange-200 hover:text-cyan-600"
      @click="$emit('cancel')"
    >
      Cancel
    </button>
    <button
      class="bg-cyan-600 text-white py-3 px-8 rounded-xl font-semibold hover:shadow-xl hover:bg-orange-200 hover:text-cyan-600"
      @click="$emit('save', selectedItems, selects)"
    >
      Save
    </button>
  </div>
</template>

<style scoped>
ul {
  list-style-type: none;
  padding-left: 0;
}
li {
  background-color: white;
  transition:
    background-color 0.3s ease,
    border-color 0.3s ease;
}
li:hover {
  background-color: #d8d1be;
}

.dualBox button {
  background-color: #69abc1;
  color: white;
  padding: 10px 15px;
  margin: 5px;
  border-radius: 10px;
}

.dualBox button:hover {
  background-color: #d8d1be;
  color: #69abc1;
}

::-webkit-scrollbar-thumb {
  background: #69abc1;
}
</style>
