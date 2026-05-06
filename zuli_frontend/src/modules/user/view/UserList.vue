<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import UserNavBar from '../components/UserNavBar.vue';
</script>

<template>
    <div class="flex flex-col">
        <PublicNavBar/>
        <UserNavBar/>
        <main class="flex-1 pb-8">
            <div class="page-shell">
                Lista de usuarios en mantenimiento
            </div>
        </main>
    </div>
</template>