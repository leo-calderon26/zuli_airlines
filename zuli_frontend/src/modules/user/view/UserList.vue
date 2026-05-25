<script setup>
import AdminNavBar from '../../../shared/AdminNavBar.vue';
import UserComponent from '../components/UserComponent.vue';
</script>

<template>
    <div class="flex flex-col">
        <AdminNavBar/>
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <UserComponent/>
            </div>
        </main>
    </div>
</template>
