<template>
  <router-link
    :to="route"
    class="group flex items-center gap-3 px-4 py-3 rounded-lg transition-colors"
    :class="isActive ? 'bg-sumary text-white' : 'text-gray-700 hover:bg-sumary hover:text-white'"
  >
    <img
      v-if="isImage"
      :src="icon"
      :alt="title"
      class="h-6 w-6 shrink-0 object-contain transition"
      :class="isActive ? 'brightness-0 invert' : 'group-hover:brightness-0 group-hover:invert'"
    />
    <component
      :is="icon"
      v-else
      class="h-6 w-6 shrink-0"
    />
    <span class="font-medium">{{ title }}</span>
  </router-link>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const props = defineProps({
  title: {
    type: String,
    required: true
  },
  icon: {
    type: [Object, Function, String],
    required: true
  },
  route: {
    type: String,
    required: true
  }
})

const currentRoute = useRoute()
const isActive = computed(() => currentRoute.path === props.route)
const isImage = computed(() => typeof props.icon === 'string')
</script>
