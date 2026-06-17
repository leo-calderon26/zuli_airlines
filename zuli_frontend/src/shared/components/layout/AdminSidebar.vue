<template>
  <div class="flex flex-col h-full w-64 bg-white border-r border-gray-200">
    <div class="flex items-center gap-3 px-6 py-5 border-b border-gray-200">
      <img src="../../../assets/logoZuli.svg" alt="Zuli Airlines" class="h-10 w-auto" />
      <div>
        <h1 class="text-lg font-bold text-gray-900">Reportes</h1>
        <p class="text-xs text-gray-500">Análisis y Estadísticas</p>
      </div>
    </div>

    <nav class="flex-1 px-4 py-6 space-y-2 overflow-y-auto">
      <SidebarMenuItem
        v-for="item in menuItems"
        :key="item.route"
        :title="item.title"
        :icon="item.icon"
        :route="item.route"
      />
    </nav>

    <div class="px-6 py-4 border-t border-gray-200">
      <p class="text-xs text-gray-500">v0.0.1</p>
    </div>
  </div>
</template>

<script setup>
import { REPORT_MENU_ITEMS } from '../../constants/admin-menu'
import SidebarMenuItem from './SidebarMenuItem.vue'

const menuItems = REPORT_MENU_ITEMS
</script>
