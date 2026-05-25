<script setup>
import AdminNavBar from '../../../shared/AdminNavBar.vue';
import AirportComponent from '../components/AirportComponent.vue';
</script>

<template>
    <div class="flex flex-col">
        <AdminNavBar/>
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <AirportComponent/>
            </div>
        </main>
    </div>
</template>